"use client";

import React, { useState, useContext } from "react";
import { ShopContext } from "@/context/shop-context";
import "./login.css";
import Button from '@mui/material/Button';


const Login = () => {
    const { loginInfo, login, logout } = useContext(ShopContext);

    const [username, setUsername] = useState("");

    const handleUsername = (e) => {
        setUsername(e.target.value);
    };

    const handleLogin = () => {
        if (username === "") return;
        login(username)
        .then((result: boolean) => {
            console.log(result);
            if (result) {
                console.log("login success");
                window.location.href = "/";
            } else {
             alert("login failed");
            }
        });
        // if(login(username)) {
        //     console.log("login success");
        //     window.location.href = "/";
        // } else {
        //     alert("login failed");
        // }
    }

    const handleLogout = () => {
        setUsername("");
        logout();
    };

    return (
        <div className="wrapper">
            <div className="content">
                {loginInfo.loggedIn ? (
                    <Button variant="contained" onClick={handleLogout}>Logout</Button>
                ) : (
                    <>
                        <div className="input">
                            User Name:
                            <input
                                type="text"
                                name="username"
                                id="username"
                                className="username"
                                onChange={handleUsername}
                            />
                        </div>
                    <Button variant="contained" onClick={handleLogin}>Login</Button>
                    </>
                )}
            </div>
        </div>
    );
};

export default Login;
