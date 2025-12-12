'use client';

import { useState, useMemo, useEffect } from 'react';
import { useAppStore } from '@/lib/store';
import Navbar from '@/components/layout/Navbar';
import { Search, SlidersHorizontal, AlertCircle, ChevronLeft, ChevronRight } from 'lucide-react';
import FilterModal, { FilterState } from '@/components/ui/FilterModal';
import GlassCard from '@/components/explore/GarmentCard';

export default function ExplorePage() {
  const { garments } = useAppStore();
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  
  // --- FILTER STATES ---
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('Tous les Costumes');
  const [filters, setFilters] = useState<FilterState>({
    maxPrice: 500,
    sort: 'Recommandé',
    conditions: [],
    availableNow: false,
  });

  // --- PAGINATION STATE ---
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 8;

  // --- FILTERING LOGIC ---
  const filteredGarments = useMemo(() => {
    return garments.filter((garment) => {
      const matchesSearch = 
        garment.title.toLowerCase().includes(searchQuery.toLowerCase()) || 
        garment.brand.toLowerCase().includes(searchQuery.toLowerCase());
      
      const matchesCategory = selectedCategory === 'Tous les Costumes' || garment.type === selectedCategory;
      const matchesPrice = garment.dailyPrice <= filters.maxPrice;
      const matchesCondition = filters.conditions.length === 0 || filters.conditions.includes(garment.condition);
      const matchesAvailability = !filters.availableNow || garment.isAvailable;

      return matchesSearch && matchesCategory && matchesPrice && matchesCondition && matchesAvailability;
    }).sort((a, b) => {
      if (filters.sort === 'Prix : Croissant') return a.dailyPrice - b.dailyPrice;
      if (filters.sort === 'Prix : Décroissant') return b.dailyPrice - a.dailyPrice;
      if (filters.sort === 'Nouveautés') return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      return 0;
    });
  }, [garments, searchQuery, selectedCategory, filters]);

  // --- PAGINATION LOGIC ---
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery, selectedCategory, filters]);

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentItems = filteredGarments.slice(indexOfFirstItem, indexOfLastItem);
  const totalPages = Math.ceil(filteredGarments.length / itemsPerPage);

  const handlePageChange = (pageNumber: number) => {
    setCurrentPage(pageNumber);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const categories = ["Tous les Costumes", "Smoking", "Costume", "Veste", "Accessoires"];

  return (
    <div className="min-h-screen bg-background text-foreground selection:bg-primary/20 transition-colors duration-300">
      <Navbar />
      
      <FilterModal 
        isOpen={isFilterOpen} 
        onClose={() => setIsFilterOpen(false)} 
        currentFilters={filters}
        onApply={setFilters}
      />

      <div className="fixed top-0 left-1/2 -translate-x-1/2 w-full h-[500px] bg-primary/10 blur-[120px] rounded-full pointer-events-none -z-10" />
      
      <main className="pt-28 px-4 md:px-8 max-w-[1800px] mx-auto pb-24">
        
        {/* Header & Search */}
        <div className="flex flex-col md:flex-row justify-between items-end mb-12 gap-6">
            <div>
                <h1 className="text-4xl md:text-5xl font-bold tracking-tight mb-2">Explorer la Collection</h1>
                <p className="text-tertiary-foreground max-w-lg text-lg">
                    Découvrez des pièces de tailleur premium sélectionnées par notre communauté.
                </p>
            </div>

            <div className="relative group w-full md:w-auto">
                <div className="absolute inset-y-0 left-4 flex items-center pointer-events-none text-tertiary-foreground group-focus-within:text-primary transition-colors">
                    <Search size={18} />
                </div>
                <input 
                    type="text" 
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    placeholder="Rechercher par marque, couleur..." 
                    className="w-full md:w-80 bg-secondary border border-tertiary rounded-full py-3 pl-12 pr-6 text-sm text-foreground placeholder:text-tertiary-foreground focus:outline-none focus:border-primary/50 focus:ring-2 focus:ring-primary/10 transition-all shadow-sm"
                />
            </div>
        </div>

        {/* Categories Bar */}
        <div className="sticky top-20 z-40 bg-background/80 backdrop-blur-xl border-y border-tertiary py-4 mb-8 -mx-4 md:-mx-8 px-4 md:px-8 flex items-center justify-between transition-colors duration-300">
            <div className="flex gap-2 overflow-x-auto no-scrollbar mask-gradient">
                {categories.map((cat) => (
                    <button 
                        key={cat}
                        onClick={() => setSelectedCategory(cat)}
                        className={`whitespace-nowrap px-5 py-2 rounded-full text-sm font-medium transition-all ${
                            selectedCategory === cat
                            ? 'bg-foreground text-background shadow-lg' 
                            : 'bg-secondary text-tertiary-foreground border border-tertiary hover:border-primary/30 hover:text-foreground'
                        }`}
                    >
                        {cat}
                    </button>
                ))}
            </div>
            
            <button 
                onClick={() => setIsFilterOpen(true)}
                className="hidden md:flex items-center gap-2 text-sm font-medium text-tertiary-foreground hover:text-primary transition-colors pl-4 border-l border-tertiary ml-4"
            >
                <SlidersHorizontal size={16} /> Filtres
                {(filters.conditions.length > 0 || filters.maxPrice < 500 || filters.availableNow) && (
                   <span className="h-2 w-2 rounded-full bg-primary ml-1"></span>
                )}
            </button>
        </div>

        {/* Results Grid */}
        {currentItems.length > 0 ? (
          <>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 min-h-[500px]">
              {currentItems.map((garment, index) => (
                <GlassCard key={garment.id} item={garment} index={index} />
              ))}
            </div>

            {/* --- PAGINATION CONTROLS --- */}
            {totalPages > 1 && (
              <div className="mt-16 flex justify-center items-center gap-4">
                <button
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={currentPage === 1}
                  className="p-3 rounded-full border border-tertiary bg-secondary text-foreground hover:bg-tertiary/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronLeft size={20} />
                </button>

                <div className="flex gap-2">
                  {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                    <button
                      key={page}
                      onClick={() => handlePageChange(page)}
                      className={`h-10 w-10 rounded-full text-sm font-bold flex items-center justify-center transition-all ${
                        currentPage === page
                          ? 'bg-primary text-background shadow-lg shadow-primary/30'
                          : 'bg-secondary text-tertiary-foreground hover:bg-tertiary/30'
                      }`}
                    >
                      {page}
                    </button>
                  ))}
                </div>

                <button
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={currentPage === totalPages}
                  className="p-3 rounded-full border border-tertiary bg-secondary text-foreground hover:bg-tertiary/50 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                >
                  <ChevronRight size={20} />
                </button>
              </div>
            )}
            
            <div className="text-center mt-4 text-xs text-tertiary-foreground">
                Affichage {indexOfFirstItem + 1}-{Math.min(indexOfLastItem, filteredGarments.length)} sur {filteredGarments.length} résultats
            </div>

          </>
        ) : (
          /* Empty State */
          <div className="flex flex-col items-center justify-center py-20 text-center">
             <div className="h-16 w-16 bg-secondary rounded-full flex items-center justify-center text-tertiary-foreground mb-4">
                <AlertCircle size={32} />
             </div>
             <h3 className="text-xl font-bold text-foreground">Aucun résultat trouvé</h3>
             <button 
               onClick={() => {
                   setSearchQuery('');
                   setFilters({ maxPrice: 500, sort: 'Recommandé', conditions: [], availableNow: false });
                   setSelectedCategory('Tous les Costumes');
               }}
               className="mt-6 text-primary font-bold hover:underline"
             >
               Réinitialiser tous les filtres
             </button>
          </div>
        )}
      </main>
    </div>
  );
}
