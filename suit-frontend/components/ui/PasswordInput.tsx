'use client';

import { useState } from 'react';
import { Eye, EyeOff } from 'lucide-react';
import { UseFormRegisterReturn } from 'react-hook-form';

interface PasswordInputProps {
  label: string;
  placeholder?: string;
  registration: UseFormRegisterReturn; // Prop to pass the {...register()}
  error?: string; // Error message if exists
}

export function PasswordInput({ label, placeholder, registration, error }: PasswordInputProps) {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="space-y-2">
      <label className="text-sm font-medium text-foreground/80 uppercase tracking-wide">
        {label}
      </label>
      
      <div className="relative">
        <input 
          {...registration}
          type={showPassword ? "text" : "password"} 
          placeholder={placeholder}
          className={`w-full bg-secondary/50 border rounded-xl px-4 py-4 pr-12 focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all placeholder:text-tertiary-foreground/50
            ${error ? 'border-red-500 focus:border-red-500' : 'border-tertiary focus:border-primary'}`}
        />
        
        {/* Toggle Button */}
        <button 
          type="button"
          onClick={() => setShowPassword(!showPassword)}
          className="absolute right-4 top-1/2 -translate-y-1/2 text-tertiary-foreground hover:text-primary transition-colors focus:outline-none"
          tabIndex={-1} // Prevents tabbing to this button while filling form
        >
          {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
        </button>
      </div>

      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
    </div>
  );
}