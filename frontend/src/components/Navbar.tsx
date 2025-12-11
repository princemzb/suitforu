import { Link } from 'react-router-dom';
import { User, LogOut } from 'lucide-react';
import { authService } from '../services/auth.service';

export const Navbar = () => {
  const isAuthenticated = authService.isAuthenticated();
  const currentUser = isAuthenticated ? authService.getCurrentUser() : null;

  const handleLogout = () => {
    authService.logout();
    window.location.href = '/';
  };

  return (
    <nav className="bg-white border-b border-gold-200 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-20">
          {/* Logo */}
          <Link to="/" className="flex items-center">
            <h1 className="text-3xl font-playfair font-bold text-navy-800">
              Suit<span className="text-gold-600">For</span>U
            </h1>
          </Link>

          {/* Navigation Links */}
          <div className="hidden md:flex items-center space-x-8">
            <Link
              to="/garments"
              className="text-navy-700 hover:text-gold-600 transition-colors font-medium"
            >
              Vêtements
            </Link>
            {isAuthenticated && (
              <>
                <Link
                  to="/my-rentals"
                  className="text-navy-700 hover:text-gold-600 transition-colors font-medium"
                >
                  Mes Locations
                </Link>
                <Link
                  to="/my-garments"
                  className="text-navy-700 hover:text-gold-600 transition-colors font-medium"
                >
                  Mes Annonces
                </Link>
              </>
            )}
          </div>

          {/* Auth Section */}
          <div className="flex items-center space-x-4">
            {isAuthenticated ? (
              <div className="flex items-center space-x-4">
                <Link
                  to="/profile"
                  className="flex items-center space-x-2 text-navy-700 hover:text-gold-600 transition-colors"
                >
                  <User size={20} />
                  <span className="hidden md:inline font-medium">
                    {currentUser?.firstName}
                  </span>
                </Link>
                <button
                  onClick={handleLogout}
                  className="flex items-center space-x-2 text-navy-700 hover:text-gold-600 transition-colors"
                  aria-label="Déconnexion"
                >
                  <LogOut size={20} />
                </button>
              </div>
            ) : (
              <div className="flex items-center space-x-4">
                <Link
                  to="/login"
                  className="text-navy-700 hover:text-gold-600 transition-colors font-medium"
                >
                  Connexion
                </Link>
                <Link
                  to="/register"
                  className="bg-gold-600 text-white px-6 py-2 rounded-lg hover:bg-gold-700 transition-colors font-medium"
                >
                  S'inscrire
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
};
