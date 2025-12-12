'use client';
import { Canvas } from '@react-three/fiber';
import { OrbitControls, Stage, useTexture } from '@react-three/drei';
import { Suspense } from 'react';

// A simple geometry that accepts a texture
function TexturedSuit({ imageUrl }: { imageUrl: string }) {
  // Load the user's image as a texture
  const texture = useTexture(imageUrl);
  
  return (
    <mesh>
      {/* Cylinder approximating a torso/suit cover */}
      <cylinderGeometry args={[1, 1, 3, 32]} />
      <meshStandardMaterial map={texture} />
    </mesh>
  );
}

export default function SuitPreview({ image }: { image: string | null }) {
  if (!image) return <div className="h-64 flex items-center justify-center border border-dashed border-gray-600 rounded-lg text-gray-400">No Image Uploaded</div>;

  return (
    <div className="h-[400px] w-full bg-neutral-900 rounded-lg overflow-hidden">
      <Canvas shadows camera={{ position: [0, 0, 5], fov: 50 }}>
        <Suspense fallback={null}>
          <Stage environment="city" intensity={0.6}>
             <TexturedSuit imageUrl={image} />
          </Stage>
        </Suspense>
        <OrbitControls autoRotate />
      </Canvas>
    </div>
  );
}