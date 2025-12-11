import { Link } from 'react-router-dom';
import { Heart, Mail, Phone, MapPin } from 'lucide-react';

export const Footer = () => {
  return (
    <footer className="bg-navy-900 text-ivory-100 mt-20">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* About */}
          <div>
            <h3 className="font-playfair text-2xl font-bold text-gold-400 mb-4">
              SuitForU
            </h3>
            <p className="text-ivory-300 text-sm">
              Location de vêtements de cérémonie entre particuliers. 
              Élégance et économie pour vos événements.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h4 className="font-playfair text-lg font-semibold mb-4">
              Liens Rapides
            </h4>
            <ul className="space-y-2 text-sm">
              <li>
                <Link to="/garments" className="hover:text-gold-400 transition-colors">
                  Vêtements
                </Link>
              </li>
              <li>
                <Link to="/about" className="hover:text-gold-400 transition-colors">
                  À Propos
                </Link>
              </li>
              <li>
                <Link to="/how-it-works" className="hover:text-gold-400 transition-colors">
                  Comment ça marche
                </Link>
              </li>
              <li>
                <Link to="/faq" className="hover:text-gold-400 transition-colors">
                  FAQ
                </Link>
              </li>
            </ul>
          </div>

          {/* Legal */}
          <div>
            <h4 className="font-playfair text-lg font-semibold mb-4">
              Légal
            </h4>
            <ul className="space-y-2 text-sm">
              <li>
                <Link to="/terms" className="hover:text-gold-400 transition-colors">
                  Conditions d'utilisation
                </Link>
              </li>
              <li>
                <Link to="/privacy" className="hover:text-gold-400 transition-colors">
                  Politique de confidentialité
                </Link>
              </li>
              <li>
                <Link to="/cookies" className="hover:text-gold-400 transition-colors">
                  Cookies
                </Link>
              </li>
            </ul>
          </div>

          {/* Contact */}
          <div>
            <h4 className="font-playfair text-lg font-semibold mb-4">
              Contact
            </h4>
            <ul className="space-y-3 text-sm">
              <li className="flex items-center space-x-2">
                <Mail size={16} className="text-gold-400" />
                <span>contact@suitforu.com</span>
              </li>
              <li className="flex items-center space-x-2">
                <Phone size={16} className="text-gold-400" />
                <span>+33 1 23 45 67 89</span>
              </li>
              <li className="flex items-center space-x-2">
                <MapPin size={16} className="text-gold-400" />
                <span>Paris, France</span>
              </li>
            </ul>
          </div>
        </div>

        {/* Bottom Bar */}
        <div className="border-t border-navy-700 mt-8 pt-8 flex flex-col md:flex-row justify-between items-center">
          <p className="text-sm text-ivory-400">
            © {new Date().getFullYear()} SuitForU. Tous droits réservés.
          </p>
          <p className="flex items-center text-sm text-ivory-400 mt-4 md:mt-0">
            Fait avec <Heart size={16} className="text-gold-400 mx-1" fill="currentColor" /> à Paris
          </p>
        </div>
      </div>
    </footer>
  );
};
