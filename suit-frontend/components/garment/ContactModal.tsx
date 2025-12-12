'use client';

import { X, Send } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useState } from 'react';
import { PublicUser } from '@/lib/store';

interface ContactModalProps {
  isOpen: boolean;
  onClose: () => void;
  owner: PublicUser;
}

export default function ContactModal({ isOpen, onClose, owner }: ContactModalProps) {
  const [message, setMessage] = useState('');
  const [sent, setSent] = useState(false);

  const handleSend = (e: React.FormEvent) => {
    e.preventDefault();
    setSent(true);
    // Simulate network request
    setTimeout(() => {
      onClose();
      setSent(false);
      setMessage('');
      alert(`Message sent to ${owner.firstName}!`);
    }, 1500);
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Backdrop */}
          <motion.div
            initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}
            onClick={onClose}
            className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[60]"
          />

          {/* Modal */}
          <motion.div
            initial={{ opacity: 0, scale: 0.95, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95, y: 20 }}
            className="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 z-[70] w-full max-w-lg bg-background border border-tertiary rounded-2xl shadow-2xl overflow-hidden"
          >
            {/* Header */}
            <div className="flex items-center justify-between p-6 border-b border-tertiary bg-secondary/30">
              <div className="flex items-center gap-3">
                 <div className="h-10 w-10 rounded-full overflow-hidden border border-tertiary">
                    <img src={owner.profilePictureUrl} alt={owner.firstName} className="h-full w-full object-cover" />
                 </div>
                 <div>
                    <h3 className="font-bold text-foreground">Contact {owner.firstName}</h3>
                    <p className="text-xs text-tertiary-foreground">Typically replies {owner.responseTime}</p>
                 </div>
              </div>
              <button onClick={onClose} className="p-2 rounded-full hover:bg-secondary text-tertiary-foreground"><X size={20} /></button>
            </div>

            {/* Body */}
            <form onSubmit={handleSend} className="p-6 space-y-4">
               <textarea 
                 value={message}
                 onChange={(e) => setMessage(e.target.value)}
                 required
                 placeholder={`Hi ${owner.firstName}, I'm interested in renting your suit for my event...`}
                 className="w-full h-40 bg-secondary/50 border border-tertiary rounded-xl p-4 focus:outline-none focus:border-primary resize-none text-foreground placeholder:text-tertiary-foreground"
               />
               
               <button 
                type="submit" 
                disabled={sent}
                className="w-full py-4 bg-foreground text-background font-bold rounded-xl hover:opacity-90 transition-all flex items-center justify-center gap-2"
               >
                 {sent ? 'Sending...' : <>Send Message <Send size={16} /></>}
               </button>
            </form>

          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}