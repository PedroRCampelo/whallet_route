import { Hero } from '../components/marketing/Hero';
import { HowItWorks } from '../components/marketing/HowItWorks';
import { ApiDocs } from '../components/marketing/ApiDocs';
import { RouteDemoSection } from '../components/route-demo/RouteDemoSection';
import { useToast } from '../components/ui/Toast';

export function LandingPage() {
  const { showToast, toastNode } = useToast();

  async function handleCopy(text: string, message: string) {
    try {
      await navigator.clipboard.writeText(text);
      showToast(message);
    } catch {
      // clipboard permission denied — nothing actionable for the user here
    }
  }

  return (
    <>
      <Hero />
      <RouteDemoSection onCopy={handleCopy} />
      <HowItWorks />
      <ApiDocs onCopy={handleCopy} />
      {toastNode}
    </>
  );
}
