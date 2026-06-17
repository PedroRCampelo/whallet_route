using WhalletRoute.Domain.Routing;

namespace WhalletRoute.Application.Routing;

public interface IRouteSolver
{
    OptimizedRoute Solve(Stop origin, IReadOnlyList<Stop> stops);
}
