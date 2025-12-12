'use client';
import { Garment } from '@/lib/type';
import Link from 'next/link';
import { motion } from 'framer-motion';
import { ArrowRight, Heart } from 'lucide-react';
import { useAppStore } from '@/lib/store';

export default function GlassCard({ item, index }: { item: Garment; index: number }) {
  const { wishlist, toggleWishlist } = useAppStore();
  const isLiked = wishlist.includes(item.id);

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.5, delay: index * 0.05 }}
      className="group relative h-[450px] w-full cursor-pointer overflow-hidden rounded-[2rem] bg-secondary shadow-sm hover:shadow-xl dark:shadow-none transition-all duration-500"
    >
        {/* 1. HEART BUTTON (Absolute Top Right) */}
        <button 
            onClick={(e) => {
                e.preventDefault(); // Prevent navigating to detail page
                e.stopPropagation();
                toggleWishlist(item.id);
            }}
            className="absolute top-4 right-4 z-20 p-2.5 rounded-full bg-background/30 backdrop-blur-md border border-white/20 hover:bg-white hover:text-red-500 transition-all shadow-sm group-hover:scale-110"
        >
            <Heart size={16} className={isLiked ? "fill-red-500 text-red-500" : "text-white"} />
        </button>

        <Link href={`/garment/${item.id}`} className="block h-full w-full">
            {/* Image Layer */}
            <div className="absolute inset-0 h-full w-full bg-tertiary">
                {/* Use item.images[0] if array, else item.image */}
                <img 
                src={item.images[0]} 
                alt={item.title}
                className="h-full w-full object-cover transition-transform duration-700 will-change-transform group-hover:scale-110"
                />
            </div>

            {/* Gradient Overlay */}
            <div className="absolute inset-0 bg-gradient-to-t from-secondary via-secondary/20 to-transparent opacity-90 transition-colors duration-300" />

            {/* Badges */}
            <div className="absolute top-4 left-4 flex gap-2">
                {item.isAvailable && (
                    <div className="flex items-center gap-1 rounded-full bg-background/50 backdrop-blur-md border border-tertiary px-3 py-1 text-[10px] font-bold uppercase tracking-widest text-emerald-500 shadow-sm">
                        <div className="h-1.5 w-1.5 rounded-full bg-emerald-500 animate-pulse" /> Available
                    </div>
                )}
            </div>

            {/* Bottom Content */}
            <div className="absolute bottom-0 left-0 right-0 p-6 transition-transform duration-500 transform translate-y-2 group-hover:translate-y-0">
                <div className="mb-2">
                    <p className="text-xs font-bold uppercase tracking-[0.2em] text-primary mb-1">{item.brand}</p>
                    <h3 className="font-serif text-2xl text-foreground leading-tight">{item.title}</h3>
                </div>

                <div className="flex items-end justify-between border-t border-tertiary pt-4 mt-4">
                    <div>
                        <p className="text-tertiary-foreground text-xs">Daily Rate</p>
                        <div className="flex items-baseline gap-1">
                            <span className="text-xl font-bold text-foreground">${item.dailyPrice}</span>
                            <span className="text-xs text-tertiary-foreground">/day</span>
                        </div>
                    </div>
                    <div className="h-10 w-10 rounded-full bg-foreground text-background flex items-center justify-center opacity-0 -translate-x-4 group-hover:opacity-100 group-hover:translate-x-0 transition-all duration-300 delay-75 shadow-lg">
                        <ArrowRight size={18} />
                    </div>
                </div>
            </div>
      </Link>
    </motion.div>
  );
}