'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useAppStore } from '@/lib/store';
import { motion } from 'framer-motion';
import { ArrowRight, Loader2 } from 'lucide-react';
import Navbar from '@/components/layout/Navbar';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { SignupSchema, SignupFormValues } from '@/lib/validation';
import { PasswordInput } from '@/components/ui/PasswordInput';


export default function SignupPage() {
  const router = useRouter();
  const login = useAppStore((state) => state.login);
  const [isLoading, setIsLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<SignupFormValues>({
    resolver: zodResolver(SignupSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      terms: false,
    },
  });

  const onSubmit = async (data: SignupFormValues) => {
    setIsLoading(true);
    setTimeout(() => {
      console.log("Signup Data:", data);
      login(data.email); // Auto-login after signup
      setIsLoading(false);
      router.push('/explore');
    }, 1500);
  };

  return (
    <div className="min-h-screen w-full flex bg-background text-foreground transition-colors duration-300">
      <Navbar />

      <div className="hidden lg:block w-1/2 relative overflow-hidden">
        <div className="absolute inset-0 bg-black/20 z-10" />
        <img 
          src="https://images.unsplash.com/photo-1617137984095-74e4e5e3613f?q=80&w=2121&auto=format&fit=crop" 
          alt="Bespoke Suit" 
          className="h-full w-full object-cover animate-slow-zoom"
        />
        <div className="absolute bottom-12 left-12 z-20 text-white max-w-md">
            <h2 className="text-4xl font-serif mb-4">"Clothes mean nothing until someone lives in them."</h2>
            <p className="text-white/80 uppercase tracking-widest text-sm">— Marc Jacobs</p>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex flex-col justify-center items-center px-8 md:px-24 pt-20">
        <motion.div 
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="w-full max-w-md space-y-8"
        >
          <div className="text-center lg:text-left">
            <h1 className="text-4xl font-bold tracking-tight mb-2">Create Account</h1>
            <p className="text-tertiary-foreground">Join the community of lenders.</p>
          </div>


          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            
            {/* Name Fields (Unchanged) */}
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground/80 uppercase tracking-wide">First Name</label>
                <input 
                  {...register("firstName")}
                  type="text" 
                  className={`w-full bg-secondary/50 border rounded-xl px-4 py-4 focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all
                  ${errors.firstName ? 'border-red-500' : 'border-tertiary focus:border-primary'}`}
                  placeholder="James" 
                />
                {errors.firstName && <p className="text-red-500 text-xs">{errors.firstName.message}</p>}
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium text-foreground/80 uppercase tracking-wide">Last Name</label>
                <input 
                  {...register("lastName")}
                  type="text" 
                  className={`w-full bg-secondary/50 border rounded-xl px-4 py-4 focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all
                  ${errors.lastName ? 'border-red-500' : 'border-tertiary focus:border-primary'}`}
                  placeholder="Bond" 
                />
                {errors.lastName && <p className="text-red-500 text-xs">{errors.lastName.message}</p>}
              </div>
            </div>

            {/* Email Field (Unchanged) */}
            <div className="space-y-2">
              <label className="text-sm font-medium text-foreground/80 uppercase tracking-wide">Email</label>
              <input 
                {...register("email")}
                type="email" 
                className={`w-full bg-secondary/50 border rounded-xl px-4 py-4 focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all
                ${errors.email ? 'border-red-500' : 'border-tertiary focus:border-primary'}`}
                placeholder="name@example.com" 
              />
              {errors.email && <p className="text-red-500 text-xs">{errors.email.message}</p>}
            </div>

            {/* NEW: Password Fields using the Reusable Component */}
            <PasswordInput 
              label="Password" 
              placeholder="Min 8 characters"
              registration={register("password")}
              error={errors.password?.message}
            />

            <PasswordInput 
              label="Confirm Password" 
              placeholder="Re-enter your password"
              registration={register("confirmPassword")}
              error={errors.confirmPassword?.message}
            />

            {/* Terms Checkbox (Unchanged) */}
            <div className="space-y-2 pt-2">
                <div className="flex items-start gap-2">
                    <input 
                      {...register("terms")}
                      type="checkbox" 
                      id="terms"
                      className="mt-1 accent-primary h-4 w-4" 
                    />
                    <label htmlFor="terms" className="text-xs text-tertiary-foreground cursor-pointer select-none">
                      I agree to the <span className="underline hover:text-primary">Terms of Service</span> and <span className="underline hover:text-primary">Privacy Policy</span>.
                    </label>
                </div>
                {errors.terms && <p className="text-red-500 text-xs">{errors.terms.message}</p>}
            </div>

            <button 
              type="submit" 
              disabled={isLoading}
              className="w-full bg-foreground text-background font-bold h-14 rounded-xl hover:opacity-90 transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4"
            >
              {isLoading ? <Loader2 className="animate-spin" /> : <>Create Account <ArrowRight size={18} /></>}
            </button>
          </form>

          <p className="text-center text-sm text-tertiary-foreground">
            Already a member? <Link href="/login" className="text-primary font-bold hover:underline">Sign In</Link>
          </p>

        </motion.div>
      </div>
    </div>
  );
}