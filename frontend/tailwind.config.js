/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        gold: {
          50: '#fdfbf7',
          100: '#f9f5e9',
          200: '#f2e8cb',
          300: '#e8d5a2',
          400: '#d4b97a',
          500: '#c9a563',
          600: '#b8924f',
          700: '#a17c42',
          800: '#85663a',
          900: '#6e5432',
        },
        ivory: {
          50: '#fefefe',
          100: '#fcfcfc',
          200: '#f8f8f6',
          300: '#f5f5f0',
          400: '#f0f0e8',
          500: '#ebe8df',
          600: '#d4cfc0',
          700: '#b5ad97',
          800: '#8f876e',
          900: '#75704f',
        },
        navy: {
          50: '#f4f6f9',
          100: '#e8ecf1',
          200: '#d7dfe9',
          300: '#bac8d9',
          400: '#99abc5',
          500: '#7f91b4',
          600: '#6a7ba4',
          700: '#5d6b94',
          800: '#4e5a7a',
          900: '#434c63',
        },
      },
      fontFamily: {
        playfair: ['"Playfair Display"', 'serif'],
        inter: ['Inter', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
