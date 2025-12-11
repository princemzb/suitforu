import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { garmentService } from '../services/garment.service';
import { GarmentCard } from '../components/GarmentCard';
import { GarmentSearchDto } from '../types/garment.types';
import { Loader2 } from 'lucide-react';

export const GarmentsList = () => {
  const [searchParams, setSearchParams] = useState<GarmentSearchDto>({
    page: 1,
    pageSize: 12,
  });

  const { data, isLoading, error } = useQuery({
    queryKey: ['garments', searchParams],
    queryFn: () => garmentService.searchGarments(searchParams),
  });

  const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    setSearchParams({
      ...searchParams,
      city: formData.get('city') as string || undefined,
      minPrice: formData.get('minPrice') ? Number(formData.get('minPrice')) : undefined,
      maxPrice: formData.get('maxPrice') ? Number(formData.get('maxPrice')) : undefined,
      size: formData.get('size') as string || undefined,
      page: 1,
    });
  };

  return (
    <div className="min-h-screen bg-ivory-50 py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Search Bar */}
        <div className="bg-white rounded-lg shadow-md p-6 mb-8">
          <h2 className="font-playfair text-2xl font-bold text-navy-800 mb-4">
            Rechercher un vêtement
          </h2>
          <form onSubmit={handleSearch} className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <input
              type="text"
              name="city"
              placeholder="Ville"
              className="border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
            />
            <input
              type="number"
              name="minPrice"
              placeholder="Prix min (€)"
              className="border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
            />
            <input
              type="number"
              name="maxPrice"
              placeholder="Prix max (€)"
              className="border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
            />
            <button
              type="submit"
              className="bg-gold-600 text-white px-6 py-2 rounded-lg hover:bg-gold-700 transition-colors font-medium"
            >
              Rechercher
            </button>
          </form>
        </div>

        {/* Results */}
        {isLoading && (
          <div className="flex justify-center items-center py-20">
            <Loader2 className="animate-spin text-gold-600" size={48} />
          </div>
        )}

        {error && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
            <p className="text-red-700">Une erreur est survenue lors du chargement des vêtements.</p>
          </div>
        )}

        {data && (
          <>
            <div className="mb-6">
              <p className="text-navy-700">
                <span className="font-semibold">{data.totalCount}</span> vêtement(s) trouvé(s)
              </p>
            </div>

            {data.items.length === 0 ? (
              <div className="bg-white rounded-lg shadow-md p-12 text-center">
                <p className="text-navy-600 text-lg">
                  Aucun vêtement ne correspond à vos critères de recherche.
                </p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {data.items.map((garment) => (
                  <GarmentCard key={garment.id} garment={garment} />
                ))}
              </div>
            )}

            {/* Pagination */}
            {data.totalPages > 1 && (
              <div className="flex justify-center items-center space-x-2 mt-8">
                {Array.from({ length: data.totalPages }, (_, i) => i + 1).map((pageNum) => (
                  <button
                    key={pageNum}
                    onClick={() => setSearchParams({ ...searchParams, page: pageNum })}
                    className={`px-4 py-2 rounded-lg transition-colors ${
                      pageNum === searchParams.page
                        ? 'bg-gold-600 text-white'
                        : 'bg-white text-navy-700 hover:bg-gold-100'
                    }`}
                  >
                    {pageNum}
                  </button>
                ))}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  );
};
