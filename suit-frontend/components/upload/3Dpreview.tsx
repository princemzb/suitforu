'use client';

import { Canvas, useFrame } from '@react-three/fiber';
import { OrbitControls, Image, Environment, MeshReflectorMaterial, Float, Text } from '@react-three/drei';
import { Suspense, useRef } from 'react';
import * as THREE from 'three';

// --- DEFINITION OF THE 6 CUBE FACES ---
// Position vector and rotation vector (Euler angles) for each face
const faceConfigs = [
  { pos: [0, 0, 2], rot: [0, 0, 0] },         // Front
  { pos: [2, 0, 0], rot: [0, Math.PI / 2, 0] }, // Right
  { pos: [0, 0, -2], rot: [0, Math.PI, 0] },    // Back
  { pos: [-2, 0, 0], rot: [0, -Math.PI / 2, 0] },// Left
  { pos: [0, 2, 0], rot: [-Math.PI / 2, 0, 0] },// Top
  { pos: [0, -2, 0], rot: [Math.PI / 2, 0, 0] },// Bottom
];

// --- INDIVIDUAL IMAGE PANEL (CUBE FACE) ---
function CubeFace({ url, index }: { url: string, index: number }) {
  const config = faceConfigs[index % 6]; // Ensure we wrap around if > 6 images (though we slice later)
  const pos = new THREE.Vector3(...config.pos);
  const rot = new THREE.Euler(...config.rot);

  return (
    <group position={pos} rotation={rot}>
        {/* The Image Panel */}
        {/* We make them slightly smaller than the grid unit (4x4) to leave gaps */}
        <Image 
          url={url} 
          transparent 
          side={THREE.DoubleSide}
          scale={[3.8, 3.8]} // Square aspect ratio to fit cube face
          toneMapped={false}
        />
        {/* Glowing border frame to define the edges */}
        <mesh position={[0,0,-0.02]}>
            <planeGeometry args={[3.9, 3.9]} />
            <meshBasicMaterial color="#4f46e5" transparent opacity={0.2} side={THREE.DoubleSide} />
        </mesh>
    </group>
  );
}

// --- THE MAIN ROTATING CUBE ASSEMBLY ---
function RotatingCubeGallery({ images }: { images: string[] }) {
    const groupRef = useRef<THREE.Group>(null);

    useFrame((state, delta) => {
        if(groupRef.current) {
            // Compound rotation on X and Y axes for dynamic tumbling effect
            groupRef.current.rotation.y += delta * 0.2;
            groupRef.current.rotation.x += delta * 0.1;
        }
    });

    // Take only the first 6 images to form the cube
    const faces = images.slice(0, 6);

    return (
        <Float speed={2} rotationIntensity={0.2} floatIntensity={0.5}>
            <group ref={groupRef}>
                {faces.map((url, i) => (
                    <CubeFace key={url + i} url={url} index={i} />
                ))}
            </group>
        </Float>
    );
}

// --- THE LUXURY POLISHED FLOOR (Unchanged) ---
function ReflectiveFloor() {
    return (
      <mesh rotation={[-Math.PI / 2, 0, 0]} position={[0, -5, 0]}>
        <planeGeometry args={[30, 30]} />
        <MeshReflectorMaterial
          blur={[400, 100]} resolution={1024} mixBlur={1} mixStrength={50} roughness={1}
          depthScale={1.2} minDepthThreshold={0.4} maxDepthThreshold={1.4}
          color="#151515" metalness={0.6} mirror={1}
        />
      </mesh>
    )
}

// --- MAIN COMPONENT ---
export default function Suit3DPreview({ image, images = [] }: { image: string | null, images?: string[] }) {
  
  let displayImages = images.length > 0 ? images : (image ? [image] : []);
  const hasUploads = displayImages.length > 0;

  // High-quality Placeholders to fill the cube if user uploads fewer than 6
  const placeholders = [
    "https://images.unsplash.com/photo-1507679799987-c73779587ccf?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80", // Suit 1
    "https://images.unsplash.com/photo-1507679799987-c73779587ccf?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80", // Suit 2
    "https://images.unsplash.com/photo-1617137984095-74e4e5e3613f?q=80&w=800", // Suit 3
    "https://images.unsplash.com/photo-1507679799987-c73779587ccf?q=80&w=800",  // Suit 4
    "https://images.unsplash.com/photo-1598808503746-f34c53b9323e?q=80&w=800",  // Suit 5
    "https://images.unsplash.com/photo-1497339100210-9e87df79c218?q=80&w=800"   // Suit 6
  ];

  // Combine uploads with placeholders to ensure we always have at least 6 for a full cube form
  // Set used to remove duplicates if user uploads the same image twice
  const finalImagesToRender = [...new Set([...displayImages, ...placeholders])].slice(0, 6);


  return (
    <div className="h-full w-full min-h-[400px] bg-secondary/30 rounded-[2rem] overflow-hidden border border-tertiary relative group">
      
      {/* Label */}
      <div className="absolute top-4 left-4 z-10 bg-background/80 backdrop-blur-md px-3 py-1 rounded-full border border-tertiary text-xs font-bold uppercase tracking-wider text-tertiary-foreground flex items-center gap-2">
        <div className="w-2 h-2 rounded-full bg-primary animate-pulse" />
        Tesseract View
      </div>

      <Canvas shadows camera={{ position: [0, 2, 12], fov: 45 }}>
        {/* Dark luxury atmosphere */}
        <color attach="background" args={['#050505']} />
        <fog attach="fog" args={['#050505', 8, 30]} />
        
        <Suspense fallback={null}>
            <Environment preset="city" />
            
            {/* The Rotating Cube */}
            <RotatingCubeGallery images={finalImagesToRender} />

            <ReflectiveFloor />

            {/* Instruction Text if empty */}
            {!hasUploads && (
                 <Float speed={2} rotationIntensity={0.2} floatIntensity={0.5}>
                    <Text 
                        position={[0, 0, 0]} 
                        fontSize={0.4} color="white" font="https://fonts.gstatic.com/s/inter/v12/UcCO3FwrK3iLTeHuS_fvQtMwCp50KnMw2boKoduKmMEVuLyfAZ9hjp-Ek-_EeA.woff"
                        anchorX="center" anchorY="middle" outlineWidth={0.02} outlineColor="#4f46e5"
                    >
                        UPLOAD IMAGES TO BUILD CUBE
                    </Text>
                 </Float>
            )}

        </Suspense>
        
        <OrbitControls 
            // Disable autoRotate because the cube itself is rotating now
            autoRotate={false}
            enableZoom={false}
            minPolarAngle={Math.PI / 2.8} 
            maxPolarAngle={Math.PI / 1.9}
        />
      </Canvas>
    </div>
  );
}