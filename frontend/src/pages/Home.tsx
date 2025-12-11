import { Link } from 'react-router-dom';
import { Search, Shield, Clock, Heart, Euro } from 'lucide-react';

export const Home = () => {
  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="relative bg-gradient-to-br from-navy-900 via-navy-800 to-gold-900 text-white py-24">
        <div className="absolute inset-0 bg-black opacity-20"></div>
        <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h1 className="font-playfair text-5xl md:text-6xl font-bold mb-6">
            Votre élégance pour chaque occasion
          </h1>
          <p className="text-xl md:text-2xl text-ivory-200 mb-8 max-w-3xl mx-auto">
            Louez ou proposez des vêtements de cérémonie entre particuliers. 
            Mariages, galas, événements... Soyez élégant sans compromis.
          </p>
          <div className="flex flex-col sm:flex-row justify-center gap-4">
            <Link
              to="/garments"
              className="bg-gold-600 text-white px-8 py-4 rounded-lg hover:bg-gold-700 transition-colors font-semibold text-lg inline-flex items-center justify-center"
            >
              <Search size={20} className="mr-2" />
              Trouver un vêtement
            </Link>
            <Link
              to="/register"
              className="bg-white text-navy-800 px-8 py-4 rounded-lg hover:bg-ivory-100 transition-colors font-semibold text-lg inline-flex items-center justify-center"
            >
              Commencer
            </Link>
          </div>
        </div>
      </section>

      {/* How It Works */}
      <section className="py-20 bg-ivory-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 className="font-playfair text-4xl font-bold text-center text-navy-800 mb-4">
            Comment ça marche ?
          </h2>
          <p className="text-center text-navy-600 mb-12 text-lg">
            Trois étapes simples pour une expérience élégante
          </p>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {/* Step 1 */}
            <div className="bg-white p-8 rounded-lg shadow-md text-center">
              <div className="w-16 h-16 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Search size={32} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-2xl font-bold text-navy-800 mb-3">
                1. Recherchez
              </h3>
              <p className="text-navy-600">
                Parcourez notre sélection de vêtements de cérémonie. 
                Filtrez par taille, style, ville et prix.
              </p>
            </div>

            {/* Step 2 */}
            <div className="bg-white p-8 rounded-lg shadow-md text-center">
              <div className="w-16 h-16 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Clock size={32} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-2xl font-bold text-navy-800 mb-3">
                2. Réservez
              </h3>
              <p className="text-navy-600">
                Choisissez vos dates, effectuez le paiement sécurisé. 
                Le propriétaire valide votre demande.
              </p>
            </div>

            {/* Step 3 */}
            <div className="bg-white p-8 rounded-lg shadow-md text-center">
              <div className="w-16 h-16 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Heart size={32} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-2xl font-bold text-navy-800 mb-3">
                3. Profitez
              </h3>
              <p className="text-navy-600">
                Récupérez le vêtement, brillez lors de votre événement, 
                puis restituez-le. Simple et élégant.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <h2 className="font-playfair text-4xl font-bold text-center text-navy-800 mb-12">
            Pourquoi choisir SuitForU ?
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
            <div className="text-center">
              <div className="w-12 h-12 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Shield size={24} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-xl font-bold text-navy-800 mb-2">
                Paiement sécurisé
              </h3>
              <p className="text-navy-600 text-sm">
                Transaction protégée par Stripe. Votre argent est en sécurité.
              </p>
            </div>

            <div className="text-center">
              <div className="w-12 h-12 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Euro size={24} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-xl font-bold text-navy-800 mb-2">
                Prix avantageux
              </h3>
              <p className="text-navy-600 text-sm">
                Jusqu'à 80% moins cher qu'un achat neuf.
              </p>
            </div>

            <div className="text-center">
              <div className="w-12 h-12 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Heart size={24} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-xl font-bold text-navy-800 mb-2">
                Économie circulaire
              </h3>
              <p className="text-navy-600 text-sm">
                Donnez une seconde vie aux vêtements de qualité.
              </p>
            </div>

            <div className="text-center">
              <div className="w-12 h-12 bg-gold-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Search size={24} className="text-gold-600" />
              </div>
              <h3 className="font-playfair text-xl font-bold text-navy-800 mb-2">
                Large choix
              </h3>
              <p className="text-navy-600 text-sm">
                Costumes, robes, accessoires... Trouvez le style parfait.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20 bg-gradient-to-r from-gold-600 to-gold-700 text-white">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="font-playfair text-4xl font-bold mb-6">
            Prêt à commencer ?
          </h2>
          <p className="text-xl mb-8 text-ivory-100">
            Inscrivez-vous gratuitement et découvrez notre catalogue de vêtements d'exception.
          </p>
          <Link
            to="/register"
            className="bg-white text-gold-700 px-8 py-4 rounded-lg hover:bg-ivory-100 transition-colors font-semibold text-lg inline-block"
          >
            Créer mon compte
          </Link>
        </div>
      </section>
    </div>
  );
};
