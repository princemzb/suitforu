'use client';
import { Canvas, useFrame } from '@react-three/fiber';
import { Environment, Float, MeshDistortMaterial, Sphere } from '@react-three/drei';
import { useRef } from 'react';
import * as THREE from 'three';

function AnimatedShape() {
  const meshRef = useRef<THREE.Mesh>(null);
  
  useFrame((state) => {
    if (meshRef.current) {
       // Gentle rotation
      meshRef.current.rotation.x = state.clock.getElapsedTime() * 0.2;
      meshRef.current.rotation.y = state.clock.getElapsedTime() * 0.3;
    }
  });

  return (
    <Sphere args={[1, 100, 200]} scale={2.4} ref={meshRef}>
      {/* This material looks like liquid fabric */}
      <MeshDistortMaterial 
        color="#4338ca" // Indigo color
        attach="material" 
        distort={0.4} 
        speed={1.5}
        roughness={0.2}
        metalness={0.8}
      />
    </Sphere>
  );
}

export default function HeroScene() {
  return (
    <div className="absolute inset-0 -z-10 opacity-50">
      <Canvas>
        <ambientLight intensity={0.5} />
        <directionalLight position={[2, 5, 2]} intensity={1} />
        <Float speed={2} rotationIntensity={1} floatIntensity={1}>
           <AnimatedShape />
        </Float>
        <Environment preset="city" />
      </Canvas>
    </div>
  );
}