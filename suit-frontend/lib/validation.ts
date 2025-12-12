import { z } from 'zod';

export const LoginSchema = z.object({
  email: z.string().email({ message: "Invalid email address" }),
  password: z.string().min(1, { message: "Password is required" }),
});

export const SignupSchema = z.object({
  firstName: z.string().min(2, { message: "First name must be at least 2 characters" }),
  lastName: z.string().min(2, { message: "Last name must be at least 2 characters" }),
  email: z.string().email({ message: "Invalid email address" }),
  password: z.string().min(8, { message: "Password must be at least 8 characters" }),
  confirmPassword: z.string().min(1, { message: "Confirm Password is required" }),
  terms: z.boolean().refine((val) => val === true, {
    message: "You must accept the terms and conditions",
  }),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords do not match",
  path: ["confirmPassword"], // This ensures the error shows under the confirm input
});

export type LoginFormValues = z.infer<typeof LoginSchema>;
export type SignupFormValues = z.infer<typeof SignupSchema>;


const MAX_FILE_SIZE = 5000000; // 5MB
const ACCEPTED_IMAGE_TYPES = ["image/jpeg", "image/jpg", "image/png", "image/webp"];

export const UploadSchema = z.object({
  title: z.string().min(5, { message: "Title must be at least 5 characters" }),
  description: z.string().min(20, { message: "Please provide more detail (min 20 chars)" }),
  brand: z.string().min(2, { message: "Brand is required" }),
  size: z.string().min(1, { message: "Size is required" }),
  type: z.string().min(1, { message: "Type is required" }),
  condition: z.enum(['Like New', 'Excellent', 'Good', 'Vintage']),
  dailyPrice: z.number().min(10, { message: "Minimum price is $10" }),
  city: z.string().min(2, { message: "City is required" }),
  // We validate the image manually in the component, but we can track if it exists here
  images: z.any(),
});

export type UploadFormValues = z.infer<typeof UploadSchema>;