'use client';

// ... (Imports restent les mêmes) ...
import { useState, useEffect, useCallback, forwardRef } from 'react';
import { useRouter } from 'next/navigation';
import { useAppStore } from '@/lib/store';
import Navbar from '@/components/layout/Navbar';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { UploadSchema, UploadFormValues } from '@/lib/validation';
import { 
  UploadCloud, Loader2, ArrowRight, X, Scan, Eye, CheckCircle2, 
  Tag, MapPin, DollarSign, Type, FileText, Ruler, ImageIcon 
} from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';

// ... (FloatingInput reste le même, juste traduire le label en français si nécessaire) ...
interface FloatingInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label: string;
  icon: React.ElementType;
  error?: string;
  prefix?: string;
}

const FloatingInput = forwardRef<HTMLInputElement, FloatingInputProps>(
  ({ label, icon: Icon, error, prefix, className, ...props }, ref) => {
    return (
      <div className="relative group">
        <div className={`
            relative flex items-center 
            bg-secondary/20 hover:bg-secondary/40 focus-within:bg-background 
            border transition-all duration-300 ease-out rounded-xl overflow-hidden
            ${error 
                ? 'border-red-500 focus-within:shadow-[0_0_0_4px_rgba(239,68,68,0.1)]' 
                : 'border-tertiary/40 hover:border-tertiary focus-within:border-primary focus-within:shadow-[0_0_0_4px_rgba(79,70,229,0.1)]'
            }
        `}>
          <div className="pl-4 text-tertiary-foreground/70 group-hover:text-tertiary-foreground group-focus-within:text-primary transition-colors duration-300">
            <Icon size={18} />
          </div>
          <div className="relative w-full">
            <input
              {...props}
              ref={ref}
              placeholder=" " 
              className={`peer w-full h-14 bg-transparent px-4 pt-4 pb-1 text-sm font-medium outline-none text-foreground placeholder-transparent ${className}`}
            />
            <label className="absolute left-4 top-1 text-[10px] font-bold uppercase tracking-wider text-tertiary-foreground/80 transition-all duration-200 
              peer-placeholder-shown:top-4 peer-placeholder-shown:text-sm peer-placeholder-shown:font-normal peer-placeholder-shown:normal-case peer-placeholder-shown:text-tertiary-foreground/60
              peer-focus:top-1 peer-focus:text-[10px] peer-focus:font-bold peer-focus:uppercase peer-focus:text-primary">
              {label}
            </label>
          </div>
          {prefix && (
             <div className="pr-4 font-bold text-tertiary-foreground peer-focus-within:text-foreground">{prefix}</div>
          )}
        </div>
        <AnimatePresence>
            {error && (
                <motion.p 
                    initial={{ opacity: 0, height: 0, y: -5 }} animate={{ opacity: 1, height: 'auto', y: 0 }} exit={{ opacity: 0, height: 0, y: -5 }}
                    className="absolute right-0 -bottom-5 text-xs text-red-500 font-medium"
                >
                    {error}
                </motion.p>
            )}
        </AnimatePresence>
      </div>
    );
  }
);
FloatingInput.displayName = 'FloatingInput';


export default function UploadPage() {
  const router = useRouter();
  const { currentUser } = useAppStore();
  
  const [previewUrls, setPreviewUrls] = useState<string[]>([]);
  const [activeImage, setActiveImage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isDragging, setIsDragging] = useState(false);

  useEffect(() => {
    if (!currentUser) router.push('/login?redirect=/upload');
  }, [currentUser, router]);

  const { register, handleSubmit, formState: { errors }, setValue, watch } = useForm<UploadFormValues>({
    resolver: zodResolver(UploadSchema),
    defaultValues: { condition: 'Like New', type: 'Suit', city: '' }
  });

  const watchedValues = watch();
  const selectedType = watch('type');

  const handleFiles = (files: FileList | null) => {
    if (files && files.length > 0) {
      const newUrls = Array.from(files).map(file => URL.createObjectURL(file));
      setPreviewUrls(prev => [...prev, ...newUrls]);
      if (!activeImage) setActiveImage(newUrls[0]);
      setValue('images', files);
    }
  };

  const handleDragOver = useCallback((e: React.DragEvent) => { e.preventDefault(); setIsDragging(true); }, []);
  const handleDragLeave = useCallback((e: React.DragEvent) => { e.preventDefault(); setIsDragging(false); }, []);
  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault(); setIsDragging(false); handleFiles(e.dataTransfer.files);
  }, []);

  const removeImage = (index: number) => {
    const urlToRemove = previewUrls[index];
    const newUrls = previewUrls.filter((_, i) => i !== index);
    setPreviewUrls(newUrls);
    if (activeImage === urlToRemove) setActiveImage(newUrls.length > 0 ? newUrls[0] : null);
  };

  const onSubmit = async (data: UploadFormValues) => {
    setIsSubmitting(true);
    setTimeout(() => { setIsSubmitting(false); router.push('/explore'); }, 2000);
  };

  if (!currentUser) return null;

  return (
    <div className="min-h-screen bg-secondary/20 text-foreground transition-colors duration-300 selection:bg-primary/20">
      <Navbar />

      <main className="pt-28 pb-44 px-4 md:px-8 max-w-6xl mx-auto">
        
        {/* Header */}
        <div className="flex flex-col items-center text-center mb-12">
            <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-primary/10 text-primary text-xs font-bold uppercase tracking-widest mb-4">
                <Scan size={14} /> Studio Créateur
            </motion.div>
            <h1 className="text-4xl md:text-5xl font-black tracking-tight mb-3">Ajoutez votre article.</h1>
            <p className="text-tertiary-foreground text-lg max-w-lg">
                Transformez votre garde-robe en source de revenus.
            </p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
          
          {/* --- COLONNE GAUCHE : ÉDITEUR --- */}
          <div className="lg:col-span-7 space-y-8">
            <div className="bg-background border border-tertiary rounded-[2rem] p-6 md:p-8 shadow-sm space-y-10">
                
                {/* 1. AJOUT DE PHOTOS */}
                <section className="space-y-6">
                   <div className="flex justify-between items-baseline border-b border-tertiary pb-4">
                       <h3 className="text-lg font-bold flex items-center gap-2"><Eye size={18} className="text-primary" /> Visuels</h3>
                       <span className="text-xs text-tertiary-foreground font-medium">Max 5 photos</span>
                   </div>
                   
                   <div 
                      onDragOver={handleDragOver} onDragLeave={handleDragLeave} onDrop={handleDrop}
                      className={`relative group border-2 border-dashed rounded-2xl p-8 flex flex-col items-center justify-center text-center transition-all duration-300 ease-out
                        ${isDragging ? 'border-primary bg-primary/5' : 'border-tertiary hover:border-primary/50 hover:bg-secondary'}`}
                   >
                        <div className={`h-12 w-12 rounded-xl flex items-center justify-center mb-4 transition-all duration-300 ${isDragging ? 'bg-primary text-white rotate-12' : 'bg-secondary text-primary'}`}>
                            <UploadCloud size={24} />
                        </div>
                        <h3 className="font-bold text-base mb-1">Glissez vos photos ici</h3>
                        <p className="text-xs text-tertiary-foreground mb-6">JPG, PNG, WEBP</p>
                        <label className="relative overflow-hidden cursor-pointer group">
                            <span className="px-6 py-3 bg-foreground text-background font-bold text-sm rounded-lg group-hover:opacity-90 transition-all inline-block shadow-md">Parcourir</span>
                            <input type="file" accept="image/*" multiple onChange={(e) => handleFiles(e.target.files)} className="absolute inset-0 w-full h-full opacity-0 cursor-pointer" />
                        </label>
                   </div>

                   <AnimatePresence>
                    {previewUrls.length > 0 && (
                        <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} className="grid grid-cols-4 sm:grid-cols-5 gap-3">
                            {previewUrls.map((url, index) => (
                            <motion.div 
                                key={url} initial={{ scale: 0.8, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} exit={{ scale: 0.5, opacity: 0 }}
                                onClick={() => setActiveImage(url)}
                                className={`relative aspect-square rounded-xl overflow-hidden cursor-pointer group transition-all duration-300 ${activeImage === url ? 'ring-2 ring-primary ring-offset-2 ring-offset-background' : 'opacity-70 hover:opacity-100'}`}
                            >
                                <img src={url} alt="Miniature" className="w-full h-full object-cover" />
                                <button type="button" onClick={(e) => { e.stopPropagation(); removeImage(index); }} className="absolute top-1 right-1 bg-black/60 text-white rounded-full p-1 opacity-0 group-hover:opacity-100 transition-opacity backdrop-blur-md"><X size={10} /></button>
                            </motion.div>
                            ))}
                        </motion.div>
                    )}
                   </AnimatePresence>
                </section>

                {/* 2. DÉTAILS */}
                <section className="space-y-6">
                    <h3 className="text-lg font-bold border-b border-tertiary pb-4 flex items-center gap-2">
                        <Scan size={18} className="text-primary" /> Détails
                    </h3>
                    
                    <div className="space-y-6">
                        <FloatingInput label="Titre de l'annonce" icon={Type} placeholder="ex. Smoking Tom Ford Bleu Marine" {...register("title")} error={errors.title?.message} />

                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                            <FloatingInput label="Marque" icon={Tag} placeholder="Gucci" {...register("brand")} error={errors.brand?.message} />
                            <FloatingInput label="Taille" icon={Ruler} placeholder="40R" {...register("size")} error={errors.size?.message} />
                        </div>
                        
                        <div className="space-y-2">
                            <label className="text-xs font-bold uppercase tracking-wider text-tertiary-foreground pl-1">Catégorie</label>
                            <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
                                {['Suit', 'Tuxedo', 'Jacket', 'Accessories'].map((type) => (
                                    <label key={type} className={`cursor-pointer border-2 rounded-xl p-3 text-center transition-all ${selectedType === type ? 'border-primary bg-primary/5 text-primary' : 'border-transparent bg-secondary/30 text-tertiary-foreground hover:bg-secondary/50'}`}>
                                        <input {...register("type")} type="radio" value={type} className="sr-only" />
                                        <span className="font-bold text-xs">{type}</span>
                                    </label>
                                ))}
                            </div>
                        </div>

                        <div className={`relative group bg-secondary/20 hover:bg-secondary/40 focus-within:bg-background border transition-all duration-300 ease-out rounded-xl overflow-hidden ${errors.description ? 'border-red-500' : 'border-tertiary/40 hover:border-tertiary focus-within:border-primary focus-within:shadow-[0_0_0_4px_rgba(79,70,229,0.1)]'}`}>
                            <div className="absolute top-4 left-4 text-tertiary-foreground/70 group-hover:text-tertiary-foreground group-focus-within:text-primary transition-colors duration-300"><FileText size={18} /></div>
                            <textarea {...register("description")} rows={4} placeholder=" " className="peer w-full bg-transparent px-4 pt-4 pb-2 pl-12 text-sm font-medium outline-none text-foreground placeholder-transparent resize-none leading-relaxed" />
                            <label className="absolute left-12 top-1 text-[10px] font-bold uppercase tracking-wider text-tertiary-foreground/80 transition-all duration-200 peer-placeholder-shown:top-4 peer-placeholder-shown:text-sm peer-placeholder-shown:font-normal peer-placeholder-shown:normal-case peer-placeholder-shown:text-tertiary-foreground/60 peer-focus:top-1 peer-focus:text-[10px] peer-focus:font-bold peer-focus:uppercase peer-focus:text-primary">Description</label>
                        </div>
                    </div>
                </section>

                {/* 3. PRIX */}
                <section className="space-y-6">
                    <h3 className="text-lg font-bold border-b border-tertiary pb-4 flex items-center gap-2">
                         <CheckCircle2 size={18} className="text-primary" /> Valeur
                    </h3>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                         <FloatingInput label="Prix par jour" icon={DollarSign} type="number" placeholder="150" {...register("dailyPrice", { valueAsNumber: true })} error={errors.dailyPrice?.message} />
                         <FloatingInput label="Ville" icon={MapPin} placeholder="Paris" {...register("city")} error={errors.city?.message} />
                    </div>
                </section>
            </div>
          </div>

          {/* --- COLONNE DROITE : APERÇU --- */}
          <div className=" lg:col-span-5 relative">
             <div className="sticky top-32">
                <div className="flex items-center gap-2 mb-4 text-primary">
                    <Eye size={18} />
                    <span className="text-sm font-bold uppercase tracking-wider">Aperçu en direct</span>
                </div>

                <div className="relative group h-[500px] w-full overflow-hidden rounded-[2rem] bg-secondary shadow-xl border border-tertiary transition-transform duration-500 hover:scale-[1.01]">
                    <div className="absolute inset-0 h-full w-full bg-tertiary">
                        {activeImage ? ( <img src={activeImage} alt="Aperçu" className="h-full w-full object-cover" /> ) : ( <div className="h-full w-full flex flex-col items-center justify-center text-tertiary-foreground gap-2"><UploadCloud size={32} /><span className="text-xs">Ajoutez des photos pour prévisualiser</span></div> )}
                    </div>
                    <div className="absolute inset-0 bg-gradient-to-t from-background via-background/20 to-transparent opacity-90" />
                    
                    <div className="absolute top-4 left-4 flex gap-2">
                        <div className="flex items-center gap-1 rounded-full bg-background/50 backdrop-blur-md border border-tertiary px-3 py-1 text-[10px] font-bold uppercase tracking-widest text-emerald-500 shadow-sm">
                            <div className="h-1.5 w-1.5 rounded-full bg-emerald-500 animate-pulse" /> Disponible
                        </div>
                    </div>
                    <div className="absolute top-4 right-4">
                         <div className="rounded-full bg-background/50 backdrop-blur-md border border-tertiary px-3 py-1 text-xs font-bold text-foreground shadow-sm">
                            {watchedValues.size || 'Taille'}
                        </div>
                    </div>

                    <div className="absolute bottom-0 left-0 right-0 p-8">
                        <div className="mb-4">
                            <p className="text-xs font-bold uppercase tracking-[0.2em] text-primary mb-1">{watchedValues.brand || 'Marque'}</p>
                            <h3 className="font-serif text-3xl text-foreground leading-tight">{watchedValues.title || 'Titre de l\'annonce'}</h3>
                        </div>
                        <div className="flex items-end justify-between border-t border-tertiary pt-4">
                            <div>
                                <p className="text-tertiary-foreground text-xs">Prix par jour</p>
                                <div className="flex items-baseline gap-1">
                                    <span className="text-2xl font-bold text-foreground">{watchedValues.dailyPrice || '0'} €</span>
                                    <span className="text-xs text-tertiary-foreground">/jour</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                
                {/* BOUTON PUBLIER (Desktop) */}
                <div className="mt-8 bg-background border border-tertiary p-5 rounded-2xl shadow-sm hidden lg:block">
                    <div className="flex justify-between items-center mb-4">
                        <p className="text-xs font-bold uppercase text-tertiary-foreground">Revenus estimés</p>
                        <p className="font-bold text-primary text-xl">~{(watchedValues.dailyPrice || 0) * 4} €/mois</p>
                    </div>
                    <button type="submit" disabled={isSubmitting} className="w-full py-4 bg-foreground text-background font-bold rounded-xl hover:opacity-90 transition-all flex items-center justify-center gap-2 shadow-lg">
                        {isSubmitting ? <Loader2 className="animate-spin" /> : <>Publier <ArrowRight size={18} /></>}
                    </button>
                </div>
             </div>
          </div>
        </form>
      </main>

      {/* --- MOBILE FIXED FOOTER --- */}
      <div className="fixed bottom-0 left-0 right-0 z-50 p-4 bg-background/80 backdrop-blur-xl border-t border-tertiary lg:hidden">
         <div className="max-w-6xl mx-auto flex items-center justify-between gap-4">
             <div>
                 <p className="text-[10px] font-bold uppercase text-tertiary-foreground">Revenus estimés</p>
                 <p className="font-bold text-primary">~{(watchedValues.dailyPrice || 0) * 4} €/mois</p>
             </div>
             <button 
                 onClick={handleSubmit(onSubmit)} 
                 disabled={isSubmitting}
                 className="flex-1 py-3 bg-foreground text-background font-bold rounded-xl hover:opacity-90 transition-all flex items-center justify-center gap-2 shadow-lg"
             >
                 {isSubmitting ? <Loader2 className="animate-spin" /> : <>Publier <ArrowRight size={18} /></>}
             </button>
         </div>
      </div>

    </div>
  );
}
