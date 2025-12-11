import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { authService } from '../services/auth.service';
import { RegisterDto } from '../types/auth.types';
import { Loader2 } from 'lucide-react';

export const Register = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<RegisterDto>({
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
  });

  const registerMutation = useMutation({
    mutationFn: (data: RegisterDto) => authService.register(data),
    onSuccess: () => {
      navigate('/');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    registerMutation.mutate(formData);
  };

  return (
    <div className="min-h-screen bg-ivory-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full">
        <div className="bg-white rounded-lg shadow-lg p-8">
          <div className="text-center mb-8">
            <h2 className="font-playfair text-3xl font-bold text-navy-800">
              Inscription
            </h2>
            <p className="mt-2 text-navy-600">
              Rejoignez la communauté SuitForU
            </p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label htmlFor="firstName" className="block text-sm font-medium text-navy-700 mb-1">
                  Prénom
                </label>
                <input
                  id="firstName"
                  type="text"
                  required
                  value={formData.firstName}
                  onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                  className="w-full border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
                />
              </div>

              <div>
                <label htmlFor="lastName" className="block text-sm font-medium text-navy-700 mb-1">
                  Nom
                </label>
                <input
                  id="lastName"
                  type="text"
                  required
                  value={formData.lastName}
                  onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                  className="w-full border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
                />
              </div>
            </div>

            <div>
              <label htmlFor="email" className="block text-sm font-medium text-navy-700 mb-1">
                Email
              </label>
              <input
                id="email"
                type="email"
                required
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                className="w-full border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
                placeholder="votre@email.com"
              />
            </div>

            <div>
              <label htmlFor="phoneNumber" className="block text-sm font-medium text-navy-700 mb-1">
                Téléphone
              </label>
              <input
                id="phoneNumber"
                type="tel"
                required
                value={formData.phoneNumber}
                onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
                className="w-full border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
                placeholder="+33 6 12 34 56 78"
              />
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-navy-700 mb-1">
                Mot de passe
              </label>
              <input
                id="password"
                type="password"
                required
                value={formData.password}
                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                className="w-full border border-ivory-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-gold-500"
                placeholder="••••••••"
              />
            </div>

            {registerMutation.error && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-3">
                <p className="text-red-700 text-sm">
                  Une erreur est survenue lors de l'inscription
                </p>
              </div>
            )}

            <button
              type="submit"
              disabled={registerMutation.isPending}
              className="w-full bg-gold-600 text-white py-3 rounded-lg hover:bg-gold-700 transition-colors font-medium flex items-center justify-center disabled:opacity-50"
            >
              {registerMutation.isPending ? (
                <>
                  <Loader2 className="animate-spin mr-2" size={20} />
                  Inscription...
                </>
              ) : (
                "S'inscrire"
              )}
            </button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-navy-600 text-sm">
              Déjà un compte ?{' '}
              <Link to="/login" className="text-gold-600 hover:text-gold-700 font-medium">
                Se connecter
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};
