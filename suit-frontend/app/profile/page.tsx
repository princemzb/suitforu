'use client';

import { useState } from 'react';
import Navbar from '@/components/layout/Navbar';
import { useAppStore } from '@/lib/store';

import { motion } from 'framer-motion';
import { 
  MapPin, Calendar, Star, Edit3, Camera, 
  Grid, MessageSquare, ShieldCheck, Mail, Save 
} from 'lucide-react';
import GlassCard from '@/components/explore/GarmentCard';

export default function ProfilePage() {
  const { currentUser, garments, reviews } = useAppStore();
  const [isEditing, setIsEditing] = useState(false);
  const [activeTab, setActiveTab] = useState<'closet' | 'reviews'>('closet');

  // État local pour l'édition
  const [formData, setFormData] = useState({
    firstName: currentUser?.firstName || '',
    lastName: currentUser?.lastName || '',
    bio: "Passionné de mode et collectionneur de pièces vintage rares. Basé à Paris.",
    location: "Paris, France"
  });

  if (!currentUser) return null;

  // Filtrer les données pour cet utilisateur
  const myGarments = garments.filter(g => g.ownerId === currentUser.id);
  const myReviews = reviews.filter(r => myGarments.map(g => g.id).includes(r.garmentId));

  return (
    <div className="min-h-screen bg-background text-foreground transition-colors duration-300 selection:bg-primary/20">
      <Navbar />

      <main className="pb-24">
        
        {/* --- HERO COVER --- */}
        <div className="relative h-64 md:h-80 w-full overflow-hidden bg-tertiary ">
            <img 
                src="https://images.unsplash.com/photo-1493238792000-8113da705763?q=80&w=2070&auto=format&fit=crop" 
                alt="Bannière" 
                className="h-full w-full object-cover opacity-60"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-background via-transparent to-transparent" />
            
            {/* Bouton Modifier la couverture */}
            {isEditing && (
                <button className="absolute top-28 right-8 bg-black/50 backdrop-blur-md text-white px-4 py-2 rounded-full text-xs font-bold flex items-center gap-2 hover:bg-black/70 transition-colors">
                    <Camera size={14} /> Changer la bannière
                </button>
            )}
        </div>

        <div className="max-w-7xl mx-auto px-4 md:px-8 relative -mt-20">
            <div className="flex flex-col md:flex-row items-start gap-8">
                
                {/* --- GAUCHE : CARTE PROFIL --- */}
                <div className="w-full md:w-80 shrink-0">
                    <div className="bg-background border border-tertiary rounded-[2rem] p-6 shadow-xl relative overflow-hidden group">
                        
                        {/* Avatar */}
                        <div className="relative mx-auto md:mx-0 w-32 h-32  mb-4">
                            <div className="h-full w-full rounded-[2rem] overflow-hidden border-4 border-background shadow-lg bg-tertiary">
                                <img src={currentUser.profilePictureUrl} alt="Profil" className="h-full w-full object-cover" />
                            </div>
                            {isEditing && (
                                <button className="absolute bottom-0 right-0 p-2 bg-primary text-white rounded-full shadow-md hover:scale-110 transition-transform">
                                    <Camera size={14} />
                                </button>
                            )}
                        </div>

                        {/* Informations utilisateur (Mode affichage vs Mode édition) */}
                        <div className="text-center md:text-left space-y-4">
                            {isEditing ? (
                                <div className="space-y-3">
                                    <div className="grid grid-cols-2 gap-2">
                                        <input 
                                            value={formData.firstName}
                                            onChange={(e) => setFormData({...formData, firstName: e.target.value})}
                                            className="w-full bg-secondary/30 border border-tertiary rounded-xl px-3 py-2 text-sm font-bold focus:border-primary outline-none"
                                        />
                                        <input 
                                            value={formData.lastName}
                                            onChange={(e) => setFormData({...formData, lastName: e.target.value})}
                                            className="w-full bg-secondary/30 border border-tertiary rounded-xl px-3 py-2 text-sm font-bold focus:border-primary outline-none"
                                        />
                                    </div>
                                    <input 
                                        value={formData.location}
                                        onChange={(e) => setFormData({...formData, location: e.target.value})}
                                        className="w-full bg-secondary/30 border border-tertiary rounded-xl px-3 py-2 text-xs font-medium focus:border-primary outline-none"
                                    />
                                    <textarea 
                                        value={formData.bio}
                                        onChange={(e) => setFormData({...formData, bio: e.target.value})}
                                        rows={3}
                                        className="w-full bg-secondary/30 border border-tertiary rounded-xl px-3 py-2 text-sm leading-relaxed focus:border-primary outline-none resize-none"
                                    />
                                    <button 
                                        onClick={() => setIsEditing(false)}
                                        className="w-full py-2 bg-primary text-white rounded-xl font-bold text-sm flex items-center justify-center gap-2"
                                    >
                                        <Save size={14} /> Enregistrer
                                    </button>
                                </div>
                            ) : (
                                <>
                                    <div>
                                        <h1 className="text-2xl font-black">{formData.firstName} {formData.lastName}</h1>
                                        <p className="text-tertiary-foreground text-sm flex items-center justify-center md:justify-start gap-1 mt-1">
                                            <MapPin size={12} /> {formData.location}
                                        </p>
                                    </div>
                                    <p className="text-sm text-foreground/80 leading-relaxed">
                                        {formData.bio}
                                    </p>
                                </>
                            )}

                            {/* Statistiques */}
                            <div className="flex items-center justify-between py-4 border-y border-tertiary">
                                <div className="text-center">
                                    <p className="text-xl font-black">{myGarments.length}</p>
                                    <p className="text-[10px] uppercase font-bold text-tertiary-foreground">Articles</p>
                                </div>
                                <div className="text-center">
                                    <p className="text-xl font-black">{myReviews.length}</p>
                                    <p className="text-[10px] uppercase font-bold text-tertiary-foreground">Avis</p>
                                </div>
                                <div className="text-center">
                                    <p className="text-xl font-black flex items-center justify-center gap-0.5">
                                        4.9 <Star size={12} className="fill-amber-400 text-amber-400" />
                                    </p>
                                    <p className="text-[10px] uppercase font-bold text-tertiary-foreground">Note</p>
                                </div>
                            </div>

                            {/* Badges de confiance */}
                            <div className="space-y-3">
                                <div className="flex items-center gap-3 text-sm">
                                    <ShieldCheck size={18} className="text-emerald-500" />
                                    <span className="font-medium">Identité vérifiée</span>
                                </div>
                                <div className="flex items-center gap-3 text-sm">
                                    <Mail size={18} className="text-primary" />
                                    <span className="font-medium">Email confirmé</span>
                                </div>
                                <div className="flex items-center gap-3 text-sm text-tertiary-foreground">
                                    <Calendar size={18} />
                                    <span>Inscrit en décembre 2023</span>
                                </div>
                            </div>
                            
                            {!isEditing && (
                                <button 
                                    onClick={() => setIsEditing(true)}
                                    className="w-full mt-4 py-3 border border-tertiary hover:bg-secondary rounded-xl font-bold text-sm flex items-center justify-center gap-2 transition-colors"
                                >
                                    <Edit3 size={14} /> Modifier le profil
                                </button>
                            )}
                        </div>
                    </div>
                </div>

                {/* --- DROITE : CONTENU DES ONGLETS --- */}
                <div className="flex-1 w-full pt-8 md:pt-20">
                    
                    {/* En-tête des onglets */}
                    <div className="flex items-center gap-6 border-b border-tertiary mb-8">
                        <button 
                            onClick={() => setActiveTab('closet')}
                            className={`pb-4 text-sm font-bold uppercase tracking-wider flex items-center gap-2 transition-colors border-b-2 ${activeTab === 'closet' ? 'border-primary text-primary' : 'border-transparent text-tertiary-foreground hover:text-foreground'}`}
                        >
                            <Grid size={16} /> Mon Dressing
                        </button>
                        <button 
                            onClick={() => setActiveTab('reviews')}
                            className={`pb-4 text-sm font-bold uppercase tracking-wider flex items-center gap-2 transition-colors border-b-2 ${activeTab === 'reviews' ? 'border-primary text-primary' : 'border-transparent text-tertiary-foreground hover:text-foreground'}`}
                        >
                            <MessageSquare size={16} /> Avis <span className="bg-secondary px-2 py-0.5 rounded-full text-[10px]">{myReviews.length}</span>
                        </button>
                    </div>

                    {/* Contenu */}
                    <div>
                        {activeTab === 'closet' && (
                            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 xl:grid-cols-3 gap-6">
                                {myGarments.map((garment, idx) => (
                                    <GlassCard key={garment.id} item={garment} index={idx} />
                                ))}
                                {myGarments.length === 0 && (
                                    <div className="col-span-full py-20 text-center border-2 border-dashed border-tertiary rounded-3xl">
                                        <p className="text-tertiary-foreground mb-4">Aucun article pour le moment.</p>
                                        <button className="text-primary font-bold hover:underline">Ajouter votre premier article</button>
                                    </div>
                                )}
                            </div>
                        )}

                        {activeTab === 'reviews' && (
                            <div className="space-y-6">
                                {myReviews.map((review) => (
                                    <div key={review.id} className="bg-background border border-tertiary p-6 rounded-2xl">
                                        <div className="flex justify-between items-start mb-4">
                                            <div className="flex items-center gap-3">
                                                <div className="h-10 w-10 bg-gradient-to-br from-indigo-400 to-purple-400 rounded-full flex items-center justify-center text-white font-bold">
                                                    {review.reviewerName.charAt(0)}
                                                </div>
                                                <div>
                                                    <p className="font-bold text-sm">{review.reviewerName}</p>
                                                    <div className="flex text-amber-400 text-xs mt-0.5">
                                                        {[...Array(5)].map((_, i) => (
                                                            <Star key={i} size={12} fill={i < review.rating ? "currentColor" : "none"} className={i >= review.rating ? "text-tertiary" : ""} />
                                                        ))}
                                                    </div>
                                                </div>
                                            </div>
                                            <span className="text-xs text-tertiary-foreground">{review.date}</span>
                                        </div>
                                        <p className="text-sm text-foreground/80 leading-relaxed">
                                            "{review.comment}"
                                        </p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                </div>
            </div>
        </div>
      </main>
    </div>
  );
}
