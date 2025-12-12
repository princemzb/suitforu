export default function GrainOverlay() {
    return (
      <div className="pointer-events-none fixed inset-0 z-[100] opacity-[0.03] mix-blend-overlay">
        <div 
          className="absolute inset-0 h-full w-full bg-repeat animate-grain"
          style={{ backgroundImage: 'url("https://grainy-gradients.vercel.app/noise.svg")' }}
        ></div>
      </div>
    );
  }