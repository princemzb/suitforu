'use client';

const categories = ["All", "Tuxedos", "Three-Piece", "Summer Linen", "Velvet", "Accessories"];

export default function CategoryFilter() {
  return (
    <div className="flex gap-8 overflow-x-auto pb-4 no-scrollbar border-b border-white/10 mb-12">
      {categories.map((cat, i) => (
        <button 
          key={cat}
          className={`whitespace-nowrap text-sm uppercase tracking-widest transition-colors ${i === 0 ? 'text-white font-bold' : 'text-neutral-500 hover:text-white'}`}
        >
          {cat}
        </button>
      ))}
    </div>
  );
}