'use client';

import Navbar from '@/components/layout/Navbar';
import { useAppStore } from '@/lib/store';
import Link from 'next/link';
import { motion, AnimatePresence } from 'framer-motion';
import { HeartOff, ArrowRight, Bookmark } from 'lucide-react';
import GlassCard from '@/components/explore/GarmentCard';

export default function WishlistPage() {
  const { garments, wishlist } = useAppStore();

  // Filtrer les articles pour n'afficher que ceux de la wishlist
  const wishlistedItems = garments.filter((item) => wishlist.includes(item.id));

  return (
    <div className="min-h-screen bg-background text-foreground transition-colors duration-300 selection:bg-primary/20">
      <Navbar />

      <main className="pt-32 pb-24 px-4 md:px-8 max-w-[1600px] mx-auto">
        
        {/* En-tête */}
        <div className="mb-12 flex items-end justify-between border-b border-tertiary pb-6">
            <div>
                <motion.div 
                    initial={{ opacity: 0, y: 10 }} 
                    animate={{ opacity: 1, y: 0 }} 
                    className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 text-primary text-xs font-bold uppercase tracking-widest mb-4"
                >
                    <Bookmark size={14} /> Votre Collection
                </motion.div>
                <h1 className="text-3xl  font-black tracking-tight">Articles enregistrés.</h1>
            </div>
            <div className="text-right hidden md:block">
                <p className="text-tertiary-foreground font-medium text-lg">{wishlistedItems.length} articles</p>
            </div>
        </div>

        {/* Contenu */}
        <AnimatePresence mode='popLayout'>
            {wishlistedItems.length > 0 ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
                    {wishlistedItems.map((garment, index) => (
                        <motion.div
                            key={garment.id}
                            layout
                            initial={{ opacity: 0, scale: 0.9 }}
                            animate={{ opacity: 1, scale: 1 }}
                            exit={{ opacity: 0, scale: 0.9, transition: { duration: 0.2 } }}
                            transition={{ duration: 0.4 }}
                        >
                            <GlassCard item={garment} index={index} />
                        </motion.div>
                    ))}
                </div>
            ) : (
                /* État vide */
                <motion.div 
                    initial={{ opacity: 0 }} 
                    animate={{ opacity: 1 }} 
                    className="flex flex-col items-center justify-center py-24 text-center border-2 border-dashed border-tertiary rounded-[3rem] bg-secondary/20"
                >
                    <div className="h-20 w-20 rounded-full bg-secondary flex items-center justify-center text-tertiary-foreground mb-6">
                        <HeartOff size={40} />
                    </div>
                    <h2 className="text-2xl font-bold mb-2">Votre wishlist est vide</h2>
                    <p className="text-tertiary-foreground max-w-md mb-8 leading-relaxed">
                        Commencez à créer votre collection personnelle. Enregistrez les articles que vous aimez pour les comparer plus tard ou les réserver pour votre prochain événement.
                    </p>
                    <Link 
                        href="/explore" 
                        className="px-8 py-4 bg-foreground text-background font-bold rounded-xl hover:opacity-90 transition-all flex items-center gap-2 shadow-xl"
                    >
                        Explorer la collection <ArrowRight size={18} />
                    </Link>
                </motion.div>
            )}
        </AnimatePresence>

      </main>
    </div>
  );
}
