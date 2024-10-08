import type { Metadata } from "next";
import { Inter } from "next/font/google";

import "./globals.css";
import { ShopContextProvider} from "@/context/shop-context";

import dynamic from "next/dynamic";

// import Navbar from './components/navbar';
// disable SSR
// https://nextjs.org/docs/messages/react-hydration-error
const Navbar = dynamic(() => import("./components/navbar"), { ssr: false });

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "ESMazon",
  description: "Nintenbo Shop",
};


export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {

  return (
    <html lang="en">

      <body className={inter.className}>
        <ShopContextProvider>
          <Navbar/>
          {children}
        </ShopContextProvider>
        </body>
    </html>
  );
}
