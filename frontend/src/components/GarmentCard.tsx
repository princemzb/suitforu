import { Link } from 'react-router-dom';
import { MapPin, Euro } from 'lucide-react';
import { Garment } from '../types/garment.types';

interface GarmentCardProps {
  garment: Garment;
}

export const GarmentCard = ({ garment }: GarmentCardProps) => {
  const primaryImage = garment.images?.find(img => img.isPrimary)?.imageUrl || 
                        garment.images?.[0]?.imageUrl ||
                        '/placeholder-garment.jpg';

  return (
    <Link
      to={`/garments/${garment.id}`}
      className="group bg-white rounded-lg overflow-hidden shadow-md hover:shadow-xl transition-all duration-300 border border-ivory-200"
    >
      {/* Image */}
      <div className="relative h-64 overflow-hidden bg-ivory-100">
        <img
          src={primaryImage}
          alt={garment.title}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        />
        <div className="absolute top-3 right-3 bg-gold-600 text-white px-3 py-1 rounded-full text-sm font-semibold">
          {garment.dailyPrice}€/jour
        </div>
      </div>

      {/* Content */}
      <div className="p-5">
        <h3 className="font-playfair text-xl font-bold text-navy-800 mb-2 group-hover:text-gold-600 transition-colors">
          {garment.title}
        </h3>
        
        <p className="text-navy-600 text-sm mb-3 line-clamp-2">
          {garment.description}
        </p>

        <div className="flex items-center justify-between text-sm">
          <div className="flex items-center space-x-1 text-navy-600">
            <MapPin size={16} className="text-gold-600" />
            <span>{garment.city}</span>
          </div>
          
          <div className="flex items-center space-x-1 text-navy-600 font-medium">
            <Euro size={16} className="text-gold-600" />
            <span>{garment.dailyPrice}/jour</span>
          </div>
        </div>

        <div className="mt-3 pt-3 border-t border-ivory-200">
          <span className="inline-block bg-ivory-100 text-navy-700 px-3 py-1 rounded-full text-xs font-medium">
            {garment.size}
          </span>
        </div>
      </div>
    </Link>
  );
};
