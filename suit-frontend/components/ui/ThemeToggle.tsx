"use client";

import * as React from "react";
import { Moon, Sun } from "lucide-react";
import { useTheme } from "next-themes";

export function ThemeToggle() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = React.useState(false);

  // Prevent hydration mismatch
  React.useEffect(() => {
    setMounted(true);
  }, []);

  console.log(theme)
  if (!mounted) return null;

  return (
    <button
      onClick={() => setTheme(theme === "dark" ? "light" : "dark")}
      // Updated classes to use semantic variables
      className="relative p-2 rounded-full border border-tertiary hover:bg-tertiary/50 hover:border-primary/50 transition-all duration-300"
      aria-label="Toggle Theme"
    >
      <div className="relative w-5 h-5">
        {/* Sun Icon (Kept Amber for visual clarity, or you can use text-primary) */}
        <Sun className={`absolute inset-0 h-[1.2rem] w-[1.2rem]  text-amber-500 ${theme == "dark" ?'hidden': 'flex'}`} />
        
        {/* Moon Icon (Now uses your Semantic Primary color) */}
        <Moon className={`absolute inset-0 h-[1.2rem] w-[1.2rem]  text-primary  ${theme == "dark" ?'flex': 'hidden'}`} />
      </div>
    </button>
  );
}