'use client';
import { motion } from 'framer-motion';

export default function InfiniteMarquee() {
  return (
    <div className="relative w-full overflow-hidden bg-white py-4 z-20">
      <div className="absolute inset-0 bg-indigo-600 mix-blend-multiply opacity-20"></div>
      <motion.div
        className="flex whitespace-nowrap"
        animate={{ x: ["0%", "-50%"] }}
        transition={{ repeat: Infinity, ease: "linear", duration: 20 }}
      >
        {[...Array(4)].map((_, i) => (
          <div key={i} className="flex items-center gap-8 px-4 text-black font-black italic tracking-tighter text-4xl uppercase">
            <span>• Rent Luxury</span>
            <span>• Own The Moment</span>
            <span>• Sustainable Style</span>
            <span>• Designer Fits</span>
          </div>
        ))}
      </motion.div>
    </div>
  );
}