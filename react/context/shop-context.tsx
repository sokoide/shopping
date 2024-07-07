"use client";
import { createContext, useEffect, useState } from "react";

// global props with the default values
export const ShopContext = createContext(null);

// const baseGoUrl = "http://localhost:8080";
const baseCsUrl = "http://localhost:5142";
const productUrl = baseCsUrl + "/products/";
const checkoutUrl = baseCsUrl + "/checkout";

const emptyCart = () => {
    const productsLength = 20;
    let items = {};
    for (let i = 1; i < productsLength + 1; i++) {
        items[i] = 0;
    }
    return items;
};

const getCart = () => {
    let items = emptyCart();

    if (typeof window !== "undefined") {
        const storedCart = localStorage.getItem("cartItems");
        return storedCart ? JSON.parse(storedCart) : items;
    }
    console.log("getCart: window NOT available, using the default empty cart");
    return items;
};

const getLoginInfo = () => {
    let loginInfo = {};

    if (typeof window !== "undefined") {
        const storedLoginInfo = localStorage.getItem("loginInfo");
        return storedLoginInfo ? JSON.parse(storedLoginInfo) : loginInfo;
    }
    console.log("getLoginInfo: window NOT available, using the empty login info");
    return loginInfo;
};

export const ShopContextProvider = (props) => {
    // *** states ***
    // state: items (products)
    const [items, setItems] = useState([]);
    useEffect(() => {
        console.log("fetching products from %O", productUrl);
        fetch(productUrl)
            .then((res) => res.json())
            .then((json) => {
                setItems(json);
            });
    }, []);

    // state: cartItems
    const [cartItems, setCartItems] = useState(getCart());
    // save cart in localStorage whenever it changes
    useEffect(() => {
        console.log("saving cartItems %O in localStorage", cartItems);
        localStorage.setItem("cartItems", JSON.stringify(cartItems));
    }, [cartItems]);

    // state: loginInfo
    const [loginInfo, setLoginInfo] = useState(getLoginInfo());
    // save loginInfo in localStorage whenever it changes
    useEffect(() => {
        console.log("saving loginInfo %O in localStorage", loginInfo);
        localStorage.setItem("loginInfo", JSON.stringify(loginInfo));
    }, [loginInfo]);

    // *** functions ***
    const getTotalCartAmount = () => {
        let totalAmount = 0;
        console.log("cart items: %O", cartItems);
        for (const item in cartItems) {
            if (cartItems[item] > 0) {
                let itemInfo = items.find(
                    (product) => product.id === Number(item)
                );
                if (itemInfo === undefined) {
                    console.error(
                        "itemInfo is null. Will be recalculated when items are set"
                    );
                    return 0;
                } else {
                    totalAmount += cartItems[item] * itemInfo.price;
                }
            }
        }
        return totalAmount;
    };

    const addToCart = (itemId) => {
        console.log("addToCart(id:%O)", itemId);
        setCartItems((prev) => ({ ...prev, [itemId]: prev[itemId] + 1 }));
    };

    const removeFromCart = (itemId) => {
        console.log("removeFromCart(id:%O)", itemId);
        setCartItems((prev) => ({ ...prev, [itemId]: prev[itemId] - 1 }));
    };

    const updateCartItemCount = (newAmount, itemId) => {
        console.log("updateCartItemCount(%O, id:%O)", newAmount, itemId);
        setCartItems((prev) => ({ ...prev, [itemId]: newAmount }));
    };

    const clearCart = () => {
        console.log("clearCart");
        setCartItems(emptyCart());
    };

    const checkout = () => {
        console.log("checkout");
        let data = {
            "cartItems": cartItems,
            "username": loginInfo.username,
        };
        console.log(
            "checking out at %O with %O",
            checkoutUrl,
            JSON.stringify(data)
        );

        fetch(checkoutUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
        })
            .then((res) => res.json())
            .then((json) => {
                console.log("json: %O", json);
                if (json.result === "Success") {
                    clearCart();
                    // show thank you message
                    alert(json.message);
                } else {
                    // TODO: show error message
                    alert("failed: " + json.message + "\nerror: " + json.error);
                }
            });
    };

    const login = (username) => {
        console.log("login(%O)", username);
        setLoginInfo((prev) => ({...prev, loggedIn: true, username: username }));
    };

    const logout = () => {
        console.log("logout");
        setLoginInfo((prev) => ({...prev, loggedIn: false, username: "" }));
    };

    const contextValue = {
        items,
        cartItems,
        loginInfo,
        getTotalCartAmount,
        addToCart,
        removeFromCart,
        updateCartItemCount,
        clearCart,
        checkout,
        login,
        logout,
    };
    return (
        <ShopContext.Provider value={contextValue}>
            {props.children}
        </ShopContext.Provider>
    );
};
