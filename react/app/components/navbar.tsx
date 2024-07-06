"use client";

import Link from "next/link";
import React, { useContext } from "react";
import "./navbar.css";
import { ShopContext } from "@/context/shop-context";


const Navbar = () => {
    const { loginInfo } = useContext(ShopContext);

    return (
        <div className="navbar">
            <div className="links">
                <Link href="/">ESMazon Nintenbo Shop</Link>
                <Link href="/cart">Cart</Link>

                <div className="user">
                    {loginInfo.loggedIn ? (
                        <Link href="/login">{loginInfo.username}</Link>
                    ) : (
                        <Link href="/login">Login</Link>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Navbar;
