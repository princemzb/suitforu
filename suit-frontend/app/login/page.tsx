'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useAppStore } from '@/lib/store';
import { motion } from 'framer-motion';
import { ArrowRight, Loader2 } from 'lucide-react';
import Navbar from '@/components/layout/Navbar';

// Imports for Form Validation
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { LoginSchema, LoginFormValues } from '@/lib/validation';
import { PasswordInput } from '@/components/ui/PasswordInput';

export default function LoginPage() {
  const router = useRouter();
  const login = useAppStore((state) => state.login);
  const [isLoading, setIsLoading] = useState(false);

  // 1. Setup React Hook Form
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(LoginSchema),
    defaultValues: {
      email: '',
      password: '',
    },
  });

  // 2. The submit handler (only runs if validation passes)
  const onSubmit = async (data: LoginFormValues) => {
    setIsLoading(true);
    
    // Simulate API delay
    setTimeout(() => {
      console.log("Login Data:", data); // Check console to see clean data
      login(data.email);
      setIsLoading(false);
      router.push('/explore');
    }, 1500);
  };

  return (
    <div className="min-h-screen w-full flex bg-background text-foreground transition-colors duration-300">
      <Navbar />

      {/* LEFT: Image Section (Unchanged) */}
      <div className="hidden lg:block w-1/2 relative overflow-hidden">
        <div className="absolute inset-0 bg-black/20 z-10" />
        <img 
          src="https://images.unsplash.com/photo-1617137984095-74e4e5e3613f?q=80&w=2121&auto=format&fit=crop" 
          alt="Luxury Suit" 
          className="h-full w-full object-cover animate-slow-zoom"
        />
        <div className="absolute bottom-12 left-12 z-20 text-white max-w-md">
            <h2 className="text-4xl font-serif mb-4">"Style is a way to say who you are without having to speak."</h2>
            <p className="text-white/80 uppercase tracking-widest text-sm">— Rachel Zoe</p>
        </div>
      </div>

      {/* RIGHT: Login Form */}
      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center px-8 md:px-24 pt-20">
        <motion.div 
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="w-full max-w-md space-y-8"
        >
          <div className="text-center lg:text-left">
            <h1 className="text-4xl font-bold tracking-tight mb-2">Welcome back</h1>
            <p className="text-tertiary-foreground">Enter your details to access your wardrobe.</p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            
            {/* Email Input */}
            <div className="space-y-2">
              <label className="text-sm font-medium text-foreground/80 uppercase tracking-wide">Email</label>
              <input 
                {...register("email")} // Link to hook form
                type="email" 
                placeholder="name@example.com"
                className={`w-full bg-secondary/50 border rounded-xl px-4 py-4 focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all placeholder:text-tertiary-foreground/50
                  ${errors.email ? 'border-red-500 focus:border-red-500' : 'border-tertiary focus:border-primary'}`}
              />
              {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email.message}</p>}
            </div>

            {/* Password Input */}
            <PasswordInput
                  label="Password" 
                  placeholder="••••••••"
                  registration={register("password")}
                  error={errors.password?.message}
               />

            <button 
              type="submit" 
              disabled={isLoading}
              className="w-full bg-foreground text-background font-bold h-14 rounded-xl hover:opacity-90 transition-all flex items-center justify-center gap-2 disabled:opacity-50"
            >
              {isLoading ? <Loader2 className="animate-spin" /> : <>Sign In <ArrowRight size={18} /></>}
            </button>
          </form>

          {/* Social Login (Keep as is) */}
          <div className="relative">
             <div className="absolute inset-0 flex items-center"><span className="w-full border-t border-tertiary"></span></div>
             <div className="relative flex justify-center text-xs uppercase"><span className="bg-background px-2 text-tertiary-foreground">Or continue with</span></div>
          </div>
          {/* ... Social Buttons ... */}
          <p className="text-center text-sm text-tertiary-foreground">
            Don't have an account? <Link href="/signup" className="text-primary font-bold hover:underline">Join the club</Link>
          </p>

        </motion.div>
      </div>
    </div>
  );
}