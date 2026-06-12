using Google.OrTools.ConstraintSolver;
using WhalletRoute.Application.Routing;
using WhalletRoute.Domain.Routing;
using WhalletRoute.Infrastructure.Geo;

namespace WhalletRoute.Infrastructure.Routing;

public sealed class OrToolsRouteSolver : IRouteSolver
{
    private const double AverageSpeedMetersPerSecond = 8.33;

    public OptimizedRoute Solve(Stop origin, IReadOnlyList<Stop> stops)
    {
        var nodes = new List<Stop> { origin };
        nodes.AddRange(stops);

        var distanceMatrix = BuildDistanceMatrix(nodes);

        var manager = new RoutingIndexManager(nodes.Count, 1, 0);
        var routing = new RoutingModel(manager);

        var transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
        {
            var fromNode = manager.IndexToNode(fromIndex);
            var toNode = manager.IndexToNode(toIndex);
            return distanceMatrix[fromNode, toNode];
        });

        routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

        var searchParameters = operations_research_constraint_solver.DefaultRoutingSearchParameters();
        searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;

        var solution = routing.SolveWithParameters(searchParameters);

        if (solution is null)
            throw new InvalidOperationException("No route solution was found.");

        return BuildRoute(nodes, manager, routing, solution);
    }

    private static long[,] BuildDistanceMatrix(IReadOnlyList<Stop> nodes)
    {
        var size = nodes.Count;
        var matrix = new long[size, size];

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                if (i == j)
                    continue;

                var meters = HaversineDistanceCalculator.DistanceInMeters(nodes[i].Coordinate, nodes[j].Coordinate);
                matrix[i, j] = (long)Math.Round(meters);
            }
        }

        return matrix;
    }

    private static OptimizedRoute BuildRoute(
        IReadOnlyList<Stop> nodes,
        RoutingIndexManager manager,
        RoutingModel routing,
        Assignment solution)
    {
        var orderedStops = new List<OrderedStop>();
        var totalDistance = 0d;
        var order = 1;

        var index = routing.Start(0);

        while (!routing.IsEnd(index))
        {
            var nextIndex = solution.Value(routing.NextVar(index));

            if (routing.IsEnd(nextIndex))
                break;

            var nextNode = manager.IndexToNode(nextIndex);
            var legDistance = (double)routing.GetArcCostForVehicle(index, nextIndex, 0);

            orderedStops.Add(new OrderedStop
            {
                StopId = nodes[nextNode].Id,
                Order = order,
                LegDistanceMeters = legDistance,
                LegDurationSeconds = legDistance / AverageSpeedMetersPerSecond
            });

            totalDistance += legDistance;
            order++;
            index = nextIndex;
        }

        return new OptimizedRoute
        {
            TotalDistanceMeters = totalDistance,
            TotalDurationSeconds = totalDistance / AverageSpeedMetersPerSecond,
            Stops = orderedStops
        };
    }
}