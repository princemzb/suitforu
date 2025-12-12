'use client';

import { Canvas, useThree } from '@react-three/fiber';
import { Image, Float, Stars, PresentationControls } from '@react-three/drei';
import { Suspense } from 'react';

function SuitCards() {
  // Get the viewport width to adjust for mobile
  const { width } = useThree((state) => state.viewport);
  const isMobile = width < 6; // Breakpoint for 3D scene

  // Responsive configurations
  const mainScale: [number, number] = isMobile ? [2.2, 3] : [3, 4];
  const sideScale: [number, number] = isMobile ? [1.8, 2.5] : [2.5, 3.5];
  
  const leftPos: [number, number, number] = isMobile ? [-1.5, -1, -2] : [-2.5, -0.5, -1];
  const rightPos: [number, number, number] = isMobile ? [1.5, 1, -2] : [2.5, 0.5, -2];

  return (
    <PresentationControls
      global
      zoom={0.8}
      rotation={[0, -0.3, 0]}
      polar={[-0.1, 0.1]}
      azimuth={[-0.1, 0.1]}
      damping={2}
      snap={true} 
    >
      <Float 
        speed={4} // Increased speed (was 1.5)
        rotationIntensity={0.5} // More rotation (was 0.2)
        floatIntensity={1.5} 
        floatingRange={[-0.1, 0.1]}
      >
        
        {/* Main Center Suit */}
        <group position={[0, 0, 0]}>
          <Image 
            url="https://images.unsplash.com/photo-1507679799987-c73779587ccf?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80"
            scale={mainScale} 
            transparent
            toneMapped={false}
            opacity={0.5}
          />
        </group>

        {/* Background Suit (Left) */}
        <group position={leftPos} rotation={[0, 0.3, 0]}>
          <Image 
            url="https://images.unsplash.com/photo-1487222477894-8943e31ef7b2?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80"
            scale={sideScale}
            toneMapped={false}
            grayscale={0.8}
            transparent
            opacity={0.6}
          />
        </group>

        {/* Background Suit (Right) */}
        <group position={rightPos} rotation={[0, -0.2, 0]}>
          <Image 
            url="https://images.unsplash.com/photo-1487222477894-8943e31ef7b2?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80"
            scale={sideScale}
            toneMapped={false}
            grayscale={0.8}
            transparent
            opacity={0.6}
          />
        </group>

      </Float>
    </PresentationControls>
  );
}

export default function SuitScene() {
  return (
    <div className="absolute inset-0 z-0">
      <Canvas camera={{ position: [0, 0, 6], fov: 45 }}>
        <ambientLight intensity={1.5} />
        
        <Suspense fallback={null}>
          <SuitCards />
        </Suspense>
        
        <Stars radius={100} depth={50} count={5000} factor={4} saturation={0} fade speed={2} />
        <fog attach="fog" args={['#0a0a0a', 5, 15]} />
      </Canvas>
    </div>
  );
}