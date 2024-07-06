import Link from "next/link";
import React from "react";
import "./navbar.css";

const Navbar =  () => {
    return (
        <div className="navbar">
            <div className="links">
                <Link href="/">ESMazon Nintenbo Shop</Link>
                {/* <Link href="/contact">Contact</Link> */}
                <Link href="/cart">Cart</Link>
            </div>
        </div>
    );
};

export default Navbar;
