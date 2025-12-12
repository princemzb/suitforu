'use client';

import { useState, useRef, useEffect } from 'react';
import Link from 'next/link';
import { 
  ShoppingBag, 
  User as UserIcon, 
  LogOut, 
  Settings, 
  ChevronDown, 
  Heart, 
  LayoutDashboard, 
  PlusCircle, 
  Package
} from 'lucide-react';
import { ThemeToggle } from '@/components/ui/ThemeToggle';
import { useAppStore } from '@/lib/store';
import { motion, AnimatePresence } from 'framer-motion';

// Helper component for consistent desktop links with animated underline
const NavLink = ({ href, children }: { href: string; children: React.ReactNode }) => (
  <Link 
    href={href} 
    className="group relative text-sm font-medium tracking-wide text-foreground/80 hover:text-foreground transition-colors"
  >
    {children}
    <span className="absolute -bottom-1 left-0 h-[2px] w-0 bg-primary transition-all duration-300 ease-out group-hover:w-full" />
  </Link>
);

export default function Navbar() {
  const { currentUser, logout, wishlist } = useAppStore();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Close dropdown on click outside
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsDropdownOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  return (
    <nav className="fixed top-0 left-0 right-0 z-50 border-b border-white/10 bg-background/80 backdrop-blur-xl transition-all duration-300">
      <div className="mx-auto flex h-20 max-w-7xl items-center justify-between px-6 lg:px-8">
        
        {/* 1. Logo */}
        <div className="flex items-center gap-2">
          <Link href="/" className="font-serif text-2xl font-bold tracking-tighter text-foreground hover:opacity-90 transition-opacity">
            Suit For You
            <span className="text-primary text-3xl leading-none">.</span>
          </Link>
        </div>
        
        {/* 2. Desktop Links */}
        <div className="hidden md:flex items-center gap-10">
          <NavLink href="/explore">Collection</NavLink>
          {currentUser && (
             <NavLink href="/upload">Ajouter un article</NavLink>
          )}
        </div>

        {/* 3. Actions */}
        <div className="flex items-center gap-5">
          <ThemeToggle />
          
          {/* Icons container for better alignment */}
          <div className="flex items-center gap-4 border-l border-foreground/10 pl-5">
            {currentUser ? (
              <div className="relative" ref={dropdownRef}>
                  
                  {/* Trigger Button */}
                  <button 
                    onClick={() => setIsDropdownOpen(!isDropdownOpen)}
                    className="flex items-center gap-3 focus:outline-none group p-1 rounded-full hover:bg-secondary/50 transition-colors"
                  >
                    <div className={`relative h-9 w-9 rounded-full overflow-hidden border-2 transition-all duration-300 ${isDropdownOpen ? 'border-primary ring-2 ring-primary/20' : 'border-transparent group-hover:border-border'}`}>
                        <img 
                          src={currentUser.profilePictureUrl} 
                          alt="Profil" 
                          className="h-full w-full object-cover" 
                        />
                    </div>
                    <ChevronDown size={14} className={`text-muted-foreground transition-transform duration-300 ${isDropdownOpen ? 'rotate-180 text-primary' : ''}`} />
                  </button>

                  {/* The Dropdown Menu */}
                  <AnimatePresence>
                    {isDropdownOpen && (
                      <motion.div
                        initial={{ opacity: 0, y: 8, scale: 0.96 }}
                        animate={{ opacity: 1, y: 0, scale: 1 }}
                        exit={{ opacity: 0, y: 8, scale: 0.96 }}
                        transition={{ duration: 0.15, ease: "easeOut" }}
                        className="absolute right-0 mt-4 w-72 origin-top-right rounded-2xl bg-background/95 backdrop-blur-2xl  shadow-2xl  overflow-hidden z-50"
                      >
                        {/* User Header */}
                        <div className="px-5 py-4 border-b border-border bg-muted/30">
                          <p className="text-sm font-bold text-foreground">{currentUser.firstName} {currentUser.lastName}</p>
                          <p className="text-xs text-muted-foreground truncate font-medium">{currentUser.email}</p>
                        </div>

                        <div className="py-2">
                          
                          {/* Section: Business (Lender) */}
                          <div className="px-5 pb-2 pt-3">
                            <span className="text-[10px] font-bold uppercase text-primary/80 tracking-widest">Espace prêteur</span>
                          </div>
                          
                          <Link 
                            href="/dashboard" 
                            onClick={() => setIsDropdownOpen(false)}
                            className="group flex items-center gap-3 px-5 py-2.5 text-sm text-foreground/80 hover:text-foreground hover:bg-primary/5 border-l-2 border-transparent hover:border-primary transition-all"
                          >
                            <LayoutDashboard size={16} className="group-hover:text-primary transition-colors" />
                            Tableau de bord
                          </Link>
                          
                          <Link 
                            href="/upload" 
                            onClick={() => setIsDropdownOpen(false)}
                            className="group flex items-center gap-3 px-5 py-2.5 text-sm text-foreground/80 hover:text-foreground hover:bg-primary/5 border-l-2 border-transparent hover:border-primary transition-all"
                          >
                            <PlusCircle size={16} className="group-hover:text-primary transition-colors" />
                            Ajouter un article
                          </Link>

                          <div className="my-2 mx-5 border-t border-border/60" />

                          {/* Section: Personal (Renter) */}
                          <div className="px-5 pb-2 pt-1">
                            <span className="text-[10px] font-bold uppercase text-muted-foreground tracking-widest">Mon compte</span>
                          </div>

                          <Link 
                            href="/rentals" 
                            onClick={() => setIsDropdownOpen(false)}
                            className="flex items-center gap-3 px-5 py-2.5 text-sm text-foreground/80 hover:bg-secondary/80 transition-colors"
                          >
                            <Package size={16} className="text-muted-foreground" />
                            Mes locations
                          </Link>

                          <Link 
                            href="/wishlist" 
                            onClick={() => setIsDropdownOpen(false)}
                            className="flex items-center gap-3 px-5 py-2.5 text-sm text-foreground/80 hover:bg-secondary/80 transition-colors"
                          >
                            <Heart size={16} className="text-muted-foreground" />
                            <span className="flex-1">Liste de souhaits</span>
                            {wishlist.length > 0 && (
                              <span className="flex h-5 w-5 items-center justify-center rounded-full bg-primary/10 text-[10px] font-bold text-primary">
                                {wishlist.length}
                              </span>
                            )}
                          </Link>
                          
                          <Link 
                            href="/profile" 
                            onClick={() => setIsDropdownOpen(false)}
                            className="flex items-center gap-3 px-5 py-2.5 text-sm text-foreground/80 hover:bg-secondary/80 transition-colors"
                          >
                            <Settings size={16} className="text-muted-foreground" />
                            Paramètres
                          </Link>
                        </div>

                        {/* Footer / Logout */}
                        <div className="bg-muted/30 p-2 border-t border-border">
                          <button 
                            onClick={() => {
                              logout();
                              setIsDropdownOpen(false);
                            }}
                            className="flex w-full items-center justify-center gap-2 px-4 py-2.5 text-xs font-bold uppercase tracking-wide text-red-500 hover:bg-red-500/10 rounded-xl transition-all"
                          >
                            <LogOut size={14} />
                            Déconnexion
                          </button>
                        </div>
                      </motion.div>
                    )}
                  </AnimatePresence>
              </div>
            ) : (
              /* Guest State */
              <Link 
                href="/login" 
                className="flex items-center gap-2 px-4 py-2 rounded-full bg-foreground text-background text-sm font-bold hover:opacity-90 transition-opacity"
              >
                  <UserIcon size={16} />
                  <span>Connexion</span>
              </Link>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}