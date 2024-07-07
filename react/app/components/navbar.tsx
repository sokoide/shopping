"use client";

import React, { useContext } from "react";
import Link from "next/link";

import "./navbar.css";
import { ShopContext } from "@/context/shop-context";

import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    IconButton,
    Box,
    ThemeProvider,
    createTheme,
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";

const Navbar = () => {
    const { loginInfo } = useContext(ShopContext);

    return (
        <AppBar position="static">
            <Toolbar>
                <IconButton
                    edge="start"
                    color="inherit"
                    aria-label="menu"
                    sx={{ mr: 2 }}
                >
                    {/* <MenuIcon /> */}
                </IconButton>
                <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                    ESMazon Nintenbo Shop
                </Typography>
                <div className="links">
                    <Link href="/">Shop</Link>
                    &nbsp;
                    <Link href="/cart">Cart</Link>
                    &nbsp;
                    <Link href="/monkey">Monkey</Link>
                    &nbsp;
                    <div className="user">
                        {loginInfo.loggedIn ? (
                            <Link href="/login">{loginInfo.username}</Link>
                        ) : (
                            <Link href="/login">Login</Link>
                        )}
                    </div>
                </div>
            </Toolbar>
        </AppBar>
    );
};

export default Navbar;
