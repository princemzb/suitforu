'use client';

import { Canvas, useFrame } from '@react-three/fiber';
import { Environment, MeshDistortMaterial, Float } from '@react-three/drei';
import { useRef } from 'react';
import * as THREE from 'three';

function FlowingFabric() {
  const meshRef = useRef<THREE.Mesh>(null);

  useFrame((state) => {
    const t = state.clock.getElapsedTime();
    if (meshRef.current) {
      // Subtle slow rotation
      meshRef.current.rotation.x = t * 0.1;
      meshRef.current.rotation.y = t * 0.15;
    }
  });

  return (
    <Float speed={1.5} rotationIntensity={0.6} floatIntensity={1}>
      <mesh ref={meshRef} scale={2.5}>
        {/* A sphere is good base for abstract flowing shapes */}
        <sphereGeometry args={[1, 128, 128]} />
        {/* MeshDistortMaterial creates the liquid/fabric effect.
           We use high metalness for a silky, reflective sheen.
        */}
        <MeshDistortMaterial
          color={"#0a0a2a"} // Deep Midnight Navy suit color
          attach="material"
          distort={0.55} // Strength of the distortion
          speed={1} // Speed of the wave animation
          roughness={0.1} // Very smooth
          metalness={0.8} // Highly reflective like silk
          side={THREE.DoubleSide}
        />
      </mesh>
    </Float>
  );
}

export default function SilkFlowScene() {
  return (
    <div className="absolute top-0 left-0 w-full h-screen -z-10 bg-neutral-950">
      <Canvas camera={{ position: [0, 0, 6], fov: 45 }}>
        {/* Dramatic Lighting */}
        <ambientLight intensity={0.2} />
        <spotLight position={[10, 10, 10]} angle={0.15} penumbra={1} intensity={1} color="#ffffff" />
        <pointLight position={[-10, -10, -10]} intensity={0.5} color="#4338ca" />

        <FlowingFabric />

        {/* City environment reflections make it look premium */}
        <Environment preset="city" />
        
        {/* Add some subtle fog to blend into the background color */}
        <fog attach="fog" args={['#0a0a0a', 5, 15]} />
      </Canvas>
    </div>
  );
}