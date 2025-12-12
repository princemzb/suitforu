'use client';

import SuitScene from '@/components/landing/SuitScene';
import Navbar from '@/components/layout/Navbar';
import Link from 'next/link';
import { motion } from 'framer-motion';
import { ArrowRight, ChevronDown, ShieldCheck, DollarSign, Sparkles } from 'lucide-react';

/* Standard Animation Variants */
const itemVariants = {
  hidden: { y: 20, opacity: 0 },
  show: { y: 0, opacity: 1, transition: { duration: 0.5 } },
};
const containerVariants = {
  hidden: { opacity: 0 },
  show: { opacity: 1, transition: { staggerChildren: 0.1 } },
};

export default function LandingPage() {
  return (
    <main className="relative w-full overflow-x-hidden min-h-screen">
      <Navbar />

      {/* --- HERO SECTION --- */}
      <section className="relative min-h-screen w-full flex flex-col justify-center overflow-hidden">
        <SuitScene />

        <div className="relative z-10 px-8 md:px-24 pointer-events-none">
          <motion.div 
            variants={containerVariants}
            initial="hidden"
            animate="show"
            className="max-w-4xl space-y-6 mt-10 md:mt-20 "
          >
            <motion.h3 variants={itemVariants} className="text-primary font-bold tracking-[0.2em] uppercase text-sm ">
              L'Échange de Costumes Premium
            </motion.h3>
            
            <motion.h1 variants={itemVariants} className="text-5xl md:text-7xl font-black tracking-tighter leading-[0.85] text-foreground mix-blend-overlay">
              HABILLEZ-VOUS.<br />
              DISTINGUEZ-VOUS.
            </motion.h1>

            <motion.p variants={itemVariants} className="text-lg md:text-xl text-secondary-foreground max-w-lg font-medium leading-relaxed pt-4">
              Louez des costumes de créateurs haut de gamme auprès de passionnés près de chez vous.
              Ayez un style exceptionnel pour une fraction du prix de vente.
            </motion.p>

            <motion.div variants={itemVariants} className="flex flex-wrap gap-4 pt-6 pointer-events-auto">
              <Link 
                href="/explore" 
                className="px-8 py-4 bg-foreground text-background font-bold rounded-full hover:opacity-90 transition-all flex items-center gap-2"
              >
                Parcourir la Collection <ArrowRight size={18} />
              </Link>
              <Link 
                href="/upload" 
                className="px-8 py-4 border border-tertiary bg-secondary/50 backdrop-blur-md text-foreground font-bold rounded-full hover:bg-secondary transition-all"
              >
                Mettre  votre costume en location
              </Link>
            </motion.div>
          </motion.div>
        </div>

        <motion.div 
          initial={{ opacity: 0 }} 
          animate={{ opacity: 1, y: [0, 10, 0] }} 
          transition={{ delay: 1, duration: 2, repeat: Infinity }}
          className="hidden absolute bottom-10 left-1/2 -translate-x-1/2 z-10 sm:flex flex-col items-center gap-2 text-tertiary-foreground"
        >
          <span className="text-[10px] uppercase tracking-widest">Découvrir davantage</span>
          <ChevronDown />
        </motion.div>
      </section>

      {/* --- HOW IT WORKS --- */}
      <section className="relative z-10 bg-secondary py-24 px-8 md:px-24 border-t border-tertiary">
        <div className="max-w-7xl mx-auto">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-12">
            
            {/* Feature 1 */}
            <div className="space-y-4">
              <div className="h-12 w-12 bg-primary/10 rounded-xl flex items-center justify-center text-primary mb-4">
                <Sparkles size={24} />
              </div>
              <h3 className="text-xl font-bold text-foreground">Marques de Luxe</h3>
              <p className="text-tertiary-foreground leading-relaxed">
                Accédez à une garde-robe sélectionnée de costumes Tom Ford, Brioni et Armani.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="space-y-4">
              <div className="h-12 w-12 bg-primary/10 rounded-xl flex items-center justify-center text-primary mb-4">
                <ShieldCheck size={24} />
              </div>
              <h3 className="text-xl font-bold text-foreground">Prêt Sécurisé</h3>
              <p className="text-tertiary-foreground leading-relaxed">
                Chaque location est assurée. Nous gérons la caution et la vérification.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="space-y-4">
              <div className="h-12 w-12 bg-primary/10 rounded-xl flex items-center justify-center text-primary mb-4">
                <DollarSign size={24} />
              </div>
              <h3 className="text-xl font-bold text-foreground">Revenus Passifs</h3>
              <p className="text-tertiary-foreground leading-relaxed">
                Listez votre costume sur la plateforme et gagnez de l'argent facilement.
              </p>
            </div>

          </div>
        </div>
      </section>
    </main>
  );
}
