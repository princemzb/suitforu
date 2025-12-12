'use client';

import { useParams, useRouter } from 'next/navigation';
import { useAppStore } from '@/lib/store';
import Navbar from '@/components/layout/Navbar';
import Link from 'next/link';
import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { ArrowLeft, Star, User, Share2, Heart, ShieldCheck, Calendar, AlertCircle, Clock, MessageCircle, CheckCircle2 } from 'lucide-react';

export default function GarmentDetail() {
  const params = useParams();
  const router = useRouter(); 
  const { garments, currentUser, getOwner } = useAppStore();

  const garment = garments.find((g) => g.id === params.id);
  
  const [selectedImage, setSelectedImage] = useState<string>(
    'https://images.unsplash.com/photo-1593030761757-71bd90d24359?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80'
  );
  
  useEffect(() => {
    if (garment && garment.images.length > 0) setSelectedImage(garment.images[0]);
  }, [garment]);

  // État de réservation
  const [days, setDays] = useState(3);
  const [startDate, setStartDate] = useState('');
  const [error, setError] = useState('');
  const [isContactOpen, setIsContactOpen] = useState(false);

  if (!garment) return <div>Non trouvé</div>;

  const rentalPrice = garment.dailyPrice * days;
  const total = rentalPrice + Math.round(rentalPrice * 0.12) + 15;

  // --- NOUVEAU HANDLER ---
  const handleReserve = () => {
    // 1. Vérifier que la date est choisie
    if (!startDate) {
      setError('Veuillez d’abord choisir une date de retrait.');
      return;
    }
    setError('');

    // 2. Vérifier l’authentification
    if (!currentUser) {
      // Redirige vers login en stockant la page actuelle
      const redirectUrl = encodeURIComponent(`/garment/${garment.id}`);
      router.push(`/login?redirect=${redirectUrl}`);
      return;
    }

    // 3. Aller au checkout avec les paramètres
    const query = new URLSearchParams({
      garmentId: garment.id,
      days: days.toString(),
      date: startDate,
      total: total.toString()
    }).toString();

    router.push(`/checkout?${query}`);
  };

  const handleMessageOwner = () => {
    if (!currentUser) {
      router.push('/login');
      return;
    }
    setIsContactOpen(true);
  };

  const owner = garment ? getOwner(garment.ownerId) : undefined;
  if (!garment || !owner) return <div>Non trouvé</div>;

  return (
    <div className="min-h-screen bg-background text-foreground transition-colors duration-300">
      <Navbar />

      <main className="pt-28 pb-20 px-4 md:px-8 max-w-[1200px] mx-auto">

        <div className="grid grid-cols-1 lg:grid-cols-9 gap-12">
          {/* Galerie (gauche) */}
          <div className="lg:col-span-5 space-y-6">
            <div className="aspect-[4/4] w-full overflow-hidden rounded-[2rem] bg-tertiary shadow-lg relative">
              <AnimatePresence mode="wait">
                <motion.img 
                  key={selectedImage}
                  initial={{ opacity: 0 }} 
                  animate={{ opacity: 1 }} 
                  exit={{ opacity: 0 }} 
                  transition={{ duration: 0.3 }}
                  src={selectedImage} 
                  alt={garment.title} 
                  className="h-full w-full object-cover absolute inset-0" 
                />
              </AnimatePresence>
            </div>
          </div>

          {/* Profil du propriétaire */}
          <div className='lg:col-span-4 flex flex-col gap-10'>
            <div className="mt-8 pt-8 border-t border-tertiary">
              <h3 className="text-lg font-bold mb-6">Rencontrez votre prêteur</h3>
              
              <div className="bg-secondary/30 border border-tertiary rounded-2xl p-6 md:p-8">
                <div className="flex flex-col md:flex-row gap-6 md:items-center justify-between">
                  
                  {/* Infos du profil */}
                  <div className="flex gap-4">
                    <div className="relative">
                      <div className="h-16 w-16 rounded-full overflow-hidden border-2 border-background shadow-lg">
                        <img src={owner.profilePictureUrl} alt={owner.firstName} className="h-full w-full object-cover" />
                      </div>
                      {owner.isVerified && (
                        <div className="absolute -bottom-1 -right-1 bg-background rounded-full p-0.5">
                          <CheckCircle2 size={20} className="text-primary fill-background" />
                        </div>
                      )}
                    </div>

                    <div>
                      <h4 className="font-bold text-xl">{owner.firstName} {owner.lastName}</h4>
                      <div className="flex items-center gap-4 text-sm text-tertiary-foreground mt-1">
                        <span className="flex items-center gap-1">
                          <Star size={14} className="fill-foreground text-foreground" /> 
                          {owner.rating} ({owner.reviews} avis)
                        </span>
                      </div>
                    </div>
                  </div>

                  {/* Bouton contacter */}
                  <button 
                    onClick={handleMessageOwner}
                    className="px-6 py-3 border border-tertiary hover:border-primary hover:text-primary rounded-xl font-bold transition-all flex items-center justify-center gap-2 bg-background"
                  >
                    <MessageCircle size={18} />
                    Contacter {owner.firstName}
                  </button>
                </div>

                {/* Statistiques */}
                <div className="grid grid-cols-2 gap-4 mt-6 pt-6 border-t border-tertiary/50">
                  <div className="flex items-center gap-3">
                    <div className="h-10 w-10 rounded-full bg-background flex items-center justify-center text-tertiary-foreground">
                      <Clock size={18} />
                    </div>
                    <div>
                      <p className="text-xs text-tertiary-foreground font-bold uppercase tracking-wider">Temps de réponse</p>
                      <p className="text-sm font-medium">{owner.responseTime}</p>
                    </div>
                  </div>

                  <div className="flex items-center gap-3">
                    <div className="h-10 w-10 rounded-full bg-background flex items-center justify-center text-tertiary-foreground">
                      <ShieldCheck size={18} />
                    </div>
                    <div>
                      <p className="text-xs text-tertiary-foreground font-bold uppercase tracking-wider">Identité</p>
                      <p className="text-sm font-medium">{owner.isVerified ? 'Prêteur vérifié' : 'Non vérifié'}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Colonne droite – Widget de réservation */}
            <div className="lg:col-span-5 relative">
              <div className="sticky top-28">
                <div className="bg-secondary/50 backdrop-blur-xl border border-tertiary p-6 md:p-8 rounded-[2rem] shadow-2xl">
                  <div className="flex justify-between items-start mb-6">
                    <div>
                      <span className="text-3xl font-bold text-foreground">${garment.dailyPrice}</span>
                      <span className="text-tertiary-foreground text-sm"> / jour</span>
                    </div>
                    <div className="flex items-center gap-1 text-sm font-bold">
                      <Star size={14} className="fill-amber-400 text-amber-400" />
                      <span>4.98</span>
                    </div>
                  </div>

                  {/* Sélecteurs de date */}
                  <div className={`grid grid-cols-2 gap-px bg-tertiary rounded-xl overflow-hidden border mb-6 transition-colors ${error ? 'border-red-500' : 'border-tertiary'}`}>
                    <div className="bg-background p-3 hover:bg-secondary transition-colors cursor-pointer relative">
                      <label className="block text-[10px] font-bold uppercase tracking-wider text-tertiary-foreground mb-1">Retrait</label>
                      <input 
                        type="date" 
                        className="w-full bg-transparent text-sm font-medium focus:outline-none text-foreground"
                        onChange={(e) => {
                          setStartDate(e.target.value);
                          setError('');
                        }}
                      />
                    </div>

                    <div className="bg-background p-3 hover:bg-secondary transition-colors cursor-pointer relative">
                      <label className="block text-[10px] font-bold uppercase tracking-wider text-tertiary-foreground mb-1">Durée</label>
                      <select 
                        value={days}
                        onChange={(e) => setDays(Number(e.target.value))}
                        className="w-full bg-transparent text-sm font-medium focus:outline-none text-foreground appearance-none cursor-pointer"
                      >
                        {[1,2,3,4,5,7,10,14].map(d => (
                          <option key={d} value={d}>{d} jours</option>
                        ))}
                      </select>
                    </div>
                  </div>

                  {/* Message d’erreur */}
                  {error && (
                    <div className="flex items-center gap-2 text-red-500 text-xs mb-4 animate-pulse">
                      <AlertCircle size={14} /> {error}
                    </div>
                  )}

                  {/* Détail du prix */}
                  <div className="space-y-3 mb-8">
                    <div className="flex justify-between text-tertiary-foreground text-sm">
                      <span className="underline decoration-dotted decoration-tertiary-foreground/30">${garment.dailyPrice} x {days} jours</span>
                      <span>${rentalPrice}</span>
                    </div>

                    <div className="flex justify-between text-tertiary-foreground text-sm">
                      <span className="underline decoration-dotted decoration-tertiary-foreground/30">Frais de service</span>
                      <span>${Math.round(rentalPrice * 0.12)}</span>
                    </div>

                    <div className="flex justify-between text-tertiary-foreground text-sm">
                      <span className="underline decoration-dotted decoration-tertiary-foreground/30">Assurance & Nettoyage</span>
                      <span>$15</span>
                    </div>

                    <div className="h-px bg-tertiary my-4" />

                    <div className="flex justify-between text-foreground font-bold text-lg">
                      <span>Total</span>
                      <span>${total}</span>
                    </div>
                  </div>

                  {/* Bouton d’action */}
                  <button 
                    onClick={handleReserve}
                    className="w-full py-4 bg-primary text-primary-foreground font-bold rounded-xl hover:opacity-90 active:scale-[0.98] transition-all flex items-center justify-center gap-2 shadow-lg shadow-primary/25"
                  >
                    Réserver pour ${total}
                  </button>

                  <p className="text-center text-xs text-tertiary-foreground mt-4 flex items-center justify-center gap-2">
                    <ShieldCheck size={12} /> Garantie Fit 100% ou remboursé
                  </p>
                </div>
              </div>
            </div>

          </div>
        </div>

      </main>
    </div>
  );
}
