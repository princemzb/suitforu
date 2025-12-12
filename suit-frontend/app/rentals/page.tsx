'use client';

import Navbar from '@/components/layout/Navbar';
import { useAppStore, Rental, RentalStatus } from '@/lib/store';
import Link from 'next/link';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  ShoppingBag, Calendar, MapPin, QrCode, 
  CheckCircle2, Circle, ArrowRight, Package, Clock, RotateCcw
} from 'lucide-react';

// --- COMPOSANT: TIMELINE D'ÉTAT ---
const StatusTimeline = ({ status }: { status: RentalStatus }) => {
  const steps = [
    { id: 'PAID', label: 'Payé', icon: CheckCircle2 },
    { id: 'PENDING_PICKUP', label: 'Prêt', icon: Package },
    { id: 'PICKED_UP', label: 'En cours', icon: Clock },
    { id: 'RETURNED', label: 'Retourné', icon: RotateCcw },
  ];

  // Trouve l'index actuel
  const currentIndex = steps.findIndex(s => s.id === status);

  return (
    <div className="flex items-center justify-between w-full mt-6 relative">
      {/* Barre de progression grise (fond) */}
      <div className="absolute top-1/2 left-0 w-full h-1 bg-tertiary -z-10 rounded-full" />
      
      {/* Barre de progression colorée (active) */}
      <div 
        className="absolute top-1/2 left-0 h-1 bg-primary -z-10 rounded-full transition-all duration-500"
        style={{ width: `${(currentIndex / (steps.length - 1)) * 100}%` }}
      />

      {steps.map((step, index) => {
        const isActive = index <= currentIndex;
        const isCurrent = index === currentIndex;
        const Icon = step.icon;

        return (
          <div key={step.id} className="flex flex-col items-center gap-2 bg-background p-1">
            <div className={`
              h-8 w-8 rounded-full flex items-center justify-center border-2 transition-all duration-300
              ${isActive ? 'bg-primary border-primary text-background' : 'bg-secondary border-tertiary text-tertiary-foreground'}
              ${isCurrent ? 'ring-4 ring-primary/20 scale-110' : ''}
            `}>
              <Icon size={14} />
            </div>
            <span className={`text-[10px] font-bold uppercase tracking-wider ${isActive ? 'text-primary' : 'text-tertiary-foreground'}`}>
              {step.label}
            </span>
          </div>
        );
      })}
    </div>
  );
};

export default function RentalsPage() {
  const { rentals, garments, updateRentalStatus } = useAppStore();

  // Fonction de démo pour faire avancer le statut quand on clique
  const advanceStatus = (rental: Rental) => {
    if (rental.status === 'PAID') updateRentalStatus(rental.id, 'PENDING_PICKUP');
    else if (rental.status === 'PENDING_PICKUP') updateRentalStatus(rental.id, 'PICKED_UP');
    else if (rental.status === 'PICKED_UP') updateRentalStatus(rental.id, 'RETURNED');
  };

  return (
    <div className="min-h-screen bg-secondary/20 text-foreground transition-colors duration-300 selection:bg-primary/20">
      <Navbar />

      <main className="pt-32 pb-24 px-4 md:px-8 max-w-5xl mx-auto">
        
        {/* Header */}
        <div className="mb-12 border-b border-tertiary pb-6 flex flex-col md:flex-row md:items-end justify-between gap-4">
            <div>
                <motion.div 
                    initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} 
                    className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 text-primary text-xs font-bold uppercase tracking-widest mb-4"
                >
                    <ShoppingBag size={14} /> My Wardrobe
                </motion.div>
                <h1 className="text-4xl md:text-5xl font-black tracking-tight">Mes Locations.</h1>
            </div>
            <p className="text-tertiary-foreground font-medium">
                {rentals.length} vêtement{rentals.length > 1 ? 's' : ''} en cours
            </p>
        </div>

        {/* LISTE DES LOCATIONS */}
        <div className="space-y-8">
            <AnimatePresence>
                {rentals.map((rental) => {
                    const garment = garments.find(g => g.id === rental.garmentId);
                    if (!garment) return null;

                    return (
                        <motion.div 
                            key={rental.id}
                            initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }}
                            className="bg-background border border-tertiary rounded-[2rem] overflow-hidden shadow-sm hover:shadow-md transition-shadow"
                        >
                            <div className="flex flex-col lg:flex-row">
                                
                                {/* Image (Gauche) */}
                                <div className="lg:w-1/3 h-64 lg:h-auto relative bg-tertiary">
                                    <img src={garment.images[0]} alt={garment.title} className="h-full w-full object-cover" />
                                    <div className="absolute top-4 left-4">
                                        <div className="bg-background/80 backdrop-blur-md px-3 py-1 rounded-full text-xs font-bold border border-tertiary">
                                            {rental.id}
                                        </div>
                                    </div>
                                </div>

                                {/* Détails (Droite) */}
                                <div className="lg:w-2/3 p-6 md:p-8 flex flex-col justify-between">
                                    
                                    {/* Info Haut */}
                                    <div className="flex justify-between items-start mb-6">
                                        <div>
                                            <p className="text-xs font-bold uppercase tracking-wider text-primary mb-1">{garment.brand}</p>
                                            <h3 className="text-2xl font-bold font-serif">{garment.title}</h3>
                                            <div className="flex items-center gap-4 mt-2 text-sm text-tertiary-foreground">
                                                <span className="flex items-center gap-1"><MapPin size={14}/> {garment.city}</span>
                                                <span className="flex items-center gap-1"><Calendar size={14}/> {rental.days} Jours</span>
                                            </div>
                                        </div>
                                        <button className="p-3 bg-secondary rounded-xl hover:bg-secondary/80 transition-colors">
                                            <QrCode size={24} />
                                        </button>
                                    </div>

                                    {/* Timeline visuelle */}
                                    <StatusTimeline status={rental.status} />

                                    {/* Footer Actions (DÉMO) */}
                                    <div className="mt-8 pt-6 border-t border-tertiary flex items-center justify-between">
                                        <div className="text-sm">
                                            <p className="text-tertiary-foreground">Total payé</p>
                                            <p className="font-bold text-lg">${rental.totalPrice}</p>
                                        </div>

                                        {/* Bouton de simulation d'état (Pour la démo) */}
                                        {rental.status !== 'RETURNED' ? (
                                            <button 
                                                onClick={() => advanceStatus(rental)}
                                                className="px-6 py-3 bg-foreground text-background font-bold rounded-xl hover:opacity-90 transition-all flex items-center gap-2"
                                            >
                                                {rental.status === 'PAID' && <>Simuler Récupération <ArrowRight size={16}/></>}
                                                {rental.status === 'PENDING_PICKUP' && <>Confirmer Pickup <Package size={16}/></>}
                                                {rental.status === 'PICKED_UP' && <>Retourner l'article <RotateCcw size={16}/></>}
                                            </button>
                                        ) : (
                                            <div className="px-6 py-3 bg-secondary text-tertiary-foreground font-bold rounded-xl border border-tertiary flex items-center gap-2">
                                                <CheckCircle2 size={16} className="text-emerald-500" />
                                                Location Terminée
                                            </div>
                                        )}
                                    </div>

                                </div>
                            </div>
                        </motion.div>
                    );
                })}
            </AnimatePresence>

            {rentals.length === 0 && (
                <div className="text-center py-20">
                    <p className="text-tertiary-foreground">Aucune location active.</p>
                    <Link href="/explore" className="text-primary font-bold hover:underline">Explorer la collection</Link>
                </div>
            )}
        </div>

      </main>
    </div>
  );
}