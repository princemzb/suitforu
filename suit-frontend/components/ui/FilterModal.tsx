'use client';

import { X, Check } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useState, useEffect } from 'react';

// Define the shape of our filter state
export interface FilterState {
  maxPrice: number;
  sort: string;
  conditions: string[];
  availableNow: boolean;
}

interface FilterModalProps {
  isOpen: boolean;
  onClose: () => void;
  currentFilters: FilterState;
  onApply: (filters: FilterState) => void;
}

export default function FilterModal({ isOpen, onClose, currentFilters, onApply }: FilterModalProps) {
  const [localFilters, setLocalFilters] = useState<FilterState>(currentFilters);

  useEffect(() => {
    if (isOpen) setLocalFilters(currentFilters);
  }, [isOpen, currentFilters]);

  const handleApply = () => {
    onApply(localFilters);
    onClose();
  };

  const toggleCondition = (c: string) => {
    const current = localFilters.conditions;
    const newConditions = current.includes(c)
      ? current.filter((i) => i !== c)
      : [...current, c];
    setLocalFilters({ ...localFilters, conditions: newConditions });
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div
            initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/40 backdrop-blur-sm z-50"
          />

          <motion.div
            initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }}
            transition={{ type: 'spring', damping: 25, stiffness: 200 }}
            className="fixed inset-y-0 right-0 z-50 w-full max-w-md bg-background border-l border-tertiary shadow-2xl flex flex-col"
          >
            <div className="flex items-center justify-between p-6 border-b border-tertiary">
              <h2 className="text-xl font-bold text-foreground">Filtres</h2>
              <button onClick={onClose} className="p-2 rounded-full hover:bg-secondary text-tertiary-foreground transition-colors"><X size={20} /></button>
            </div>

            <div className="flex-1 overflow-y-auto p-6 space-y-8">
              
              {/* PRICE */}
              <div className="space-y-4">
                <div className="flex justify-between items-center">
                   <h3 className="font-bold text-foreground">Prix quotidien max</h3>
                   <span className="text-primary font-mono font-bold">{localFilters.maxPrice}€</span>
                </div>
                <input 
                  type="range" min="50" max="500" step="10"
                  value={localFilters.maxPrice}
                  onChange={(e) => setLocalFilters({ ...localFilters, maxPrice: parseInt(e.target.value) })}
                  className="w-full h-2 bg-secondary rounded-lg appearance-none cursor-pointer accent-primary"
                />
              </div>

              {/* SORT */}
              <div className="space-y-3">
                <h3 className="font-bold text-foreground">Trier par</h3>
                <div className="space-y-2">
                   {[
                     'Recommandé',
                     'Prix : Croissant',
                     'Prix : Décroissant',
                     'Nouveautés'
                   ].map((option) => (
                     <label key={option} className="flex items-center gap-3 cursor-pointer group">
                        <div className={`w-5 h-5 rounded-full border flex items-center justify-center transition-colors ${localFilters.sort === option ? 'border-primary bg-primary' : 'border-tertiary bg-secondary'}`}>
                           {localFilters.sort === option && <Check size={12} className="text-background" />}
                        </div>
                        <input 
                          type="radio" name="sort" className="hidden"
                          checked={localFilters.sort === option}
                          onChange={() => setLocalFilters({ ...localFilters, sort: option })}
                        />
                        <span className={`text-sm ${localFilters.sort === option ? 'text-foreground font-medium' : 'text-tertiary-foreground'}`}>{option}</span>
                     </label>
                   ))}
                </div>
              </div>

              {/* CONDITION */}
              <div className="space-y-3">
                <h3 className="font-bold text-foreground">État</h3>
                <div className="flex flex-wrap gap-2">
                   {['Comme neuf', 'Excellent', 'Bon', 'Vintage'].map((cond) => (
                     <button
                       key={cond}
                       onClick={() => toggleCondition(cond)}
                       className={`px-4 py-2 rounded-full text-sm font-medium border transition-all ${
                         localFilters.conditions.includes(cond)
                           ? 'bg-foreground text-background border-foreground'
                           : 'bg-secondary text-tertiary-foreground border-tertiary hover:border-primary/50'
                       }`}
                     >
                       {cond}
                     </button>
                   ))}
                </div>
              </div>

              {/* AVAILABILITY */}
              <div className="flex items-center justify-between p-4 rounded-xl border border-tertiary bg-secondary/50">
                 <div>
                    <h4 className="font-bold text-foreground text-sm">Disponible maintenant</h4>
                    <p className="text-xs text-tertiary-foreground">Afficher uniquement les articles disponibles aujourd'hui</p>
                 </div>
                 <label className="relative inline-flex items-center cursor-pointer">
                    <input 
                        type="checkbox" 
                        className="sr-only peer"
                        checked={localFilters.availableNow}
                        onChange={(e) => setLocalFilters({...localFilters, availableNow: e.target.checked})}
                    />
                    <div className="w-11 h-6 bg-tertiary peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary"></div>
                 </label>
              </div>
            </div>

            <div className="p-6 border-t border-tertiary bg-secondary/30 backdrop-blur-md flex gap-4">
               <button 
                onClick={() => setLocalFilters({ maxPrice: 500, sort: 'Recommandé', conditions: [], availableNow: false })}
                className="flex-1 py-3 px-4 rounded-xl border border-tertiary text-foreground font-bold hover:bg-secondary transition-colors"
               >
                 Réinitialiser
               </button>
               <button 
                onClick={handleApply}
                className="flex-1 py-3 px-4 rounded-xl bg-foreground text-background font-bold hover:opacity-90 transition-opacity"
               >
                 Afficher les résultats
               </button>
            </div>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
