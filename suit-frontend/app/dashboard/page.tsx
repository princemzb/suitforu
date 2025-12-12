'use client';

import { useState } from 'react';
import Navbar from '@/components/layout/Navbar';
import { useAppStore, RentalStatus } from '@/lib/store';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  LayoutDashboard, Inbox, Shirt, History, Star, 
  CheckCircle2, XCircle, Clock, Calendar, DollarSign, 
  ChevronRight, MoreHorizontal, Power
} from 'lucide-react';
import Link from 'next/link';

export default function DashboardPage() {
  const { currentUser, garments, rentals, reviews, updateRentalStatus, toggleAvailability } = useAppStore();
  const [activeTab, setActiveTab] = useState<'overview' | 'requests' | 'active' | 'closet' | 'reviews'>('overview');

  // FILTRAGE DES DONNÉES
  const myGarments = garments.filter(g => g.ownerId === currentUser?.id);
  const myGarmentIds = myGarments.map(g => g.id);
  
  const myRentals = rentals.filter(r => myGarmentIds.includes(r.garmentId));
  
  const pendingRentals = myRentals.filter(r => r.status === 'PENDING');
  const activeRentals = myRentals.filter(r => ['APPROVED', 'PAID', 'PICKED_UP'].includes(r.status));
  const historyRentals = myRentals.filter(r => ['RETURNED', 'COMPLETED', 'REJECTED'].includes(r.status));
  const myReviews = reviews.filter(r => myGarmentIds.includes(r.garmentId));

  const totalEarnings = historyRentals.reduce((acc, curr) => acc + curr.totalPrice, 0);

  const StatCard = ({ title, value, icon: Icon, color }: any) => (
    <div className="bg-background border border-tertiary p-6 rounded-2xl flex items-center gap-4 shadow-sm">
        <div className={`h-12 w-12 rounded-xl flex items-center justify-center ${color}`}>
            <Icon size={24} />
        </div>
        <div>
            <p className="text-xs font-bold uppercase text-tertiary-foreground">{title}</p>
            <h3 className="text-2xl font-black">{value}</h3>
        </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-secondary/20 text-foreground selection:bg-primary/20">
      <Navbar />

      <main className="pt-32 pb-24 px-4 md:px-8 max-w-7xl mx-auto">
        
        {/* HEADER */}
        <div className="flex flex-col md:flex-row justify-between items-end mb-12 gap-4">
            <div>
                <h1 className="text-4xl font-black mb-2">Tableau de bord prêteur</h1>
                <p className="text-tertiary-foreground">Gérez votre activité de location.</p>
            </div>
            <div className="flex items-center gap-2 bg-background border border-tertiary px-4 py-2 rounded-full shadow-sm">
                <div className="h-2 w-2 rounded-full bg-emerald-500 animate-pulse" />
                <span className="text-sm font-bold">Compte actif</span>
            </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
            
            {/* NAVIGATION LATÉRALE */}
            <div className="lg:col-span-3 sticky top-32 space-y-2 bg-background">
                {[
                    { id: 'overview', label: 'Aperçu', icon: LayoutDashboard },
                    { id: 'requests', label: 'Demandes', icon: Inbox, badge: pendingRentals.length },
                    { id: 'active', label: 'Locations en cours', icon: Clock, badge: activeRentals.length },
                    { id: 'closet', label: 'Mon Dressing', icon: Shirt },
                    { id: 'reviews', label: 'Avis', icon: Star },
                ].map((item) => (
                    <button
                        key={item.id}
                        onClick={() => setActiveTab(item.id as any)}
                        className={`w-full flex items-center justify-between px-4 py-3 rounded-xl transition-all font-medium ${
                            activeTab === item.id 
                            ? 'bg-primary text-white shadow-lg shadow-primary/20' 
                            : 'bg-background hover:bg-secondary border border-transparent hover:border-tertiary text-tertiary-foreground hover:text-foreground'
                        }`}
                    >
                        <div className="flex items-center gap-3">
                            <item.icon size={18} />
                            <span>{item.label}</span>
                        </div>
                        {item.badge ? (
                            <span className="bg-white/20 text-xs px-2 py-0.5 rounded-full font-bold">{item.badge}</span>
                        ) : null}
                    </button>
                ))}
            </div>

            {/* CONTENU PRINCIPAL */}
            <div className="lg:col-span-9 space-y-6">
                
                {/* --- APERÇU --- */}
                {activeTab === 'overview' && (
                    <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="space-y-8">
                        
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                            <StatCard title="Revenus Totaux" value={`$${totalEarnings}`} icon={DollarSign} color="bg-emerald-500/10 text-emerald-500" />
                            <StatCard title="Locations Totales" value={historyRentals.length + activeRentals.length} icon={History} color="bg-blue-500/10 text-blue-500" />
                            <StatCard title="Note Moyenne" value="4.9" icon={Star} color="bg-amber-500/10 text-amber-500" />
                        </div>
                        
                        <div className="bg-background border border-tertiary rounded-2xl p-6">
                            <h3 className="font-bold text-lg mb-4">Activité récente</h3>
                            {historyRentals.length > 0 ? (
                                <div className="space-y-4">
                                    {historyRentals.slice(0, 3).map((r) => (
                                        <div key={r.id} className="flex items-center justify-between border-b border-tertiary/50 pb-4 last:border-0 last:pb-0">
                                            <div className="flex items-center gap-4">
                                                <div className="h-10 w-10 bg-secondary rounded-full flex items-center justify-center font-bold text-tertiary-foreground">
                                                    {r.renterName.charAt(0)}
                                                </div>
                                                <div>
                                                    <p className="font-bold text-sm">{r.renterName}</p>
                                                    <p className="text-xs text-tertiary-foreground">Retourné le {r.startDate}</p>
                                                </div>
                                            </div>
                                            <span className="text-emerald-500 font-bold text-sm">+${r.totalPrice}</span>
                                        </div>
                                    ))}
                                </div>
                            ) : (
                                <p className="text-tertiary-foreground text-sm">Aucune activité récente.</p>
                            )}
                        </div>
                    </motion.div>
                )}

                {/* --- DEMANDES --- */}
                {activeTab === 'requests' && (
                    <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
                        <h2 className="text-xl font-bold mb-6">Demandes en attente ({pendingRentals.length})</h2>

                        {pendingRentals.length > 0 ? (
                            <div className="grid gap-4">
                                {pendingRentals.map((rental) => {
                                    const item = garments.find(g => g.id === rental.garmentId);
                                    return (
                                        <div key={rental.id} className="bg-background border border-tertiary p-6 rounded-2xl flex flex-col md:flex-row md:items-center justify-between gap-6 shadow-sm">
                                            <div className="flex items-start gap-4">
                                                {/* Image */}
                                                <div className="h-16 w-16 bg-tertiary rounded-xl overflow-hidden shrink-0">
                                                    {item?.images && <img src={item.images[0]} className="h-full w-full object-cover" />}
                                                </div>

                                                {/* Infos */}
                                                <div>
                                                    <h3 className="font-bold text-lg">{item?.title}</h3>
                                                    <div className="flex items-center gap-4 text-sm text-tertiary-foreground mt-1">
                                                        <span className="flex items-center gap-1"><Inbox size={14}/> {rental.renterName}</span>
                                                        <span className="flex items-center gap-1"><Calendar size={14}/> {rental.startDate} ({rental.days} jours)</span>
                                                    </div>
                                                    <p className="text-primary font-bold mt-2">Gain potentiel : ${rental.totalPrice}</p>
                                                </div>
                                            </div>
                                            
                                            {/* Actions */}
                                            <div className="flex gap-3">
                                                <button 
                                                    onClick={() => updateRentalStatus(rental.id, 'REJECTED')}
                                                    className="px-4 py-2 border border-tertiary hover:bg-red-50 text-red-600 font-bold rounded-xl transition-colors flex items-center gap-2"
                                                >
                                                    <XCircle size={18} /> Refuser
                                                </button>
                                                <button 
                                                    onClick={() => updateRentalStatus(rental.id, 'APPROVED')}
                                                    className="px-6 py-2 bg-foreground text-background font-bold rounded-xl hover:opacity-90 transition-colors flex items-center gap-2 shadow-lg"
                                                >
                                                    <CheckCircle2 size={18} /> Accepter
                                                </button>
                                            </div>
                                        </div>
                                    )
                                })}
                            </div>
                        ) : (
                            <div className="text-center py-12 bg-background border border-tertiary rounded-2xl">
                                <div className="h-16 w-16 bg-secondary rounded-full flex items-center justify-center mx-auto mb-4 text-tertiary-foreground"><Inbox size={32}/></div>
                                <p className="text-tertiary-foreground">Toutes les demandes ont été traitées !</p>
                            </div>
                        )}
                    </motion.div>
                )}

                {/* --- MON DRESSING --- */}
                {activeTab === 'closet' && (
                    <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
                         <div className="flex justify-between items-center mb-6">
                            <h2 className="text-xl font-bold">Mon Dressing ({myGarments.length})</h2>
                            <Link href={"/upload"} className="text-sm font-bold text-primary hover:underline">+ Ajouter un article</Link>
                        </div>
                        <div className="grid gap-4">
                            {myGarments.map((garment) => (
                                <div key={garment.id} className={`bg-background border rounded-2xl p-4 flex items-center justify-between transition-colors ${garment.isAvailable ? 'border-tertiary' : 'border-red-200 bg-red-50/50'}`}>
                                    <div className="flex items-center gap-4">
                                        <div className="h-14 w-14 bg-tertiary rounded-xl overflow-hidden grayscale-[50%]">
                                             {garment.images && <img src={garment.images[0]} className="h-full w-full object-cover" />}
                                        </div>
                                        <div>
                                            <h3 className="font-bold">{garment.title}</h3>
                                            <p className="text-xs text-tertiary-foreground">${garment.dailyPrice}/jour</p>
                                        </div>
                                    </div>
                                    
                                    {/* Disponibilité */}
                                    <button 
                                        onClick={() => toggleAvailability(garment.id)}
                                        className={`flex items-center gap-2 px-4 py-2 rounded-full text-xs font-bold transition-all ${
                                            garment.isAvailable 
                                            ? 'bg-emerald-100 text-emerald-700 hover:bg-emerald-200' 
                                            : 'bg-neutral-200 text-neutral-600 hover:bg-neutral-300'
                                        }`}
                                    >
                                        <Power size={14} />
                                        {garment.isAvailable ? 'Disponible' : 'En pause'}
                                    </button>
                                </div>
                            ))}
                        </div>
                    </motion.div>
                )}

                {/* --- AVIS --- */}
                {activeTab === 'reviews' && (
                    <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
                         <h2 className="text-xl font-bold mb-6">Avis récents</h2>
                         <div className="space-y-4">
                            {myReviews && myReviews.map((review) => (
                                <div key={review.id} className="bg-background border border-tertiary p-6 rounded-2xl">
                                    <div className="flex justify-between items-start mb-2">
                                        <div className="flex items-center gap-3">
                                            <div className="h-10 w-10 bg-gradient-to-br from-indigo-400 to-purple-400 rounded-full flex items-center justify-center text-white font-bold">
                                                {review.reviewerName.charAt(0)}
                                            </div>
                                            <div>
                                                <p className="font-bold text-sm">{review.reviewerName}</p>
                                                <div className="flex text-amber-400 text-xs">
                                                    {[...Array(5)].map((_, i) => (
                                                        <Star 
                                                            key={i} 
                                                            size={12} 
                                                            fill={i < review.rating ? "currentColor" : "none"} 
                                                            className={i >= review.rating ? "text-tertiary" : ""} 
                                                        />
                                                    ))}
                                                </div>
                                            </div>
                                        </div>
                                        <span className="text-xs text-tertiary-foreground">{review.date}</span>
                                    </div>

                                    <p className="text-sm text-foreground/80 leading-relaxed pl-14">
                                        "{review.comment}"
                                    </p>
                                </div>
                            ))}
                         </div>
                    </motion.div>
                )}

            </div>
        </div>

      </main>
    </div>
  );
}
