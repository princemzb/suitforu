import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useMutation } from '@tanstack/react-query';
import { authService } from '../services/auth.service';
import { LoginDto } from '../types/auth.types';
import { Loader2 } from 'lucide-react';

export const Login = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<LoginDto>({
    email: '',
    password: '',
  });

  const loginMutation = useMutation({
    mutationFn: (data: LoginDto) => authService.login(data),
    onSuccess: () => {
      navigate('/');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    loginMutation.mutate(formData);
  };

  return (
    <div className="min-h-screen bg-ivory-50 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full">
        <div className="bg-white rounded-lg shadow-lg p-8">
          <div className="text-center mb-8">
            <h2 className="font-playfair text-3xl font-bold text-navy-800">
              Connexion
            </h2>
            <p className="mt-2 text-navy-600">
              Bienvenue sur SuitForU
            </p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
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

            {loginMutation.error && (
              <div className="bg-red-50 border border-red-200 rounded-lg p-3">
                <p className="text-red-700 text-sm">
                  Email ou mot de passe incorrect
                </p>
              </div>
            )}

            <button
              type="submit"
              disabled={loginMutation.isPending}
              className="w-full bg-gold-600 text-white py-3 rounded-lg hover:bg-gold-700 transition-colors font-medium flex items-center justify-center disabled:opacity-50"
            >
              {loginMutation.isPending ? (
                <>
                  <Loader2 className="animate-spin mr-2" size={20} />
                  Connexion...
                </>
              ) : (
                'Se connecter'
              )}
            </button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-navy-600 text-sm">
              Pas encore de compte ?{' '}
              <Link to="/register" className="text-gold-600 hover:text-gold-700 font-medium">
                S'inscrire
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};
