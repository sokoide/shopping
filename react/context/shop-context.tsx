"use client";
import { createContext, useEffect, useState } from "react";

// global props with the default values
export const ShopContext = createContext(null);

//const productUrl = "https://fakestoreapi.com/products";
const baseUrl = "http://localhost:8080";
const productUrl = baseUrl + "/products";
const checkoutUrl = baseUrl + "/checkout";

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

export const ShopContextProvider = (props) => {
    const [items, setItems] = useState([]);
    useEffect(() => {
        console.log( "fetching products from %O", productUrl);
        fetch(productUrl)
            .then((res) => res.json())
            .then((json) => {
                setItems(json);
            });
    }, []);

    const [cartItems, setCartItems] = useState(getCart());
    // save cart to localStorage whenever it changes
    useEffect(() => {
        console.log("saving cartItems %O in localStorage", cartItems);
        localStorage.setItem("cartItems", JSON.stringify(cartItems));
    }, [cartItems]);

    const getTotalCartAmount = () => {
        let totalAmount = 0;
        console.log("cart items: %O", cartItems);
        for (const item in cartItems) {
            console.log("item: %O", item);
            if (cartItems[item] > 0) {
                let itemInfo = items.find(
                    (product) => product.id === Number(item)
                );
                if (itemInfo === undefined) {
                    console.error( "itemInfo is null. Will be recalculated when items are set");
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

    const emptyCart = () => {
        console.log("emtpyCart");
        setCartItems(emptyCart());
    };

    const checkout = () => {
        console.log("checkout");
        console.log( "checking out at %O with %O", checkoutUrl, JSON.stringify(cartItems));
        fetch(checkoutUrl, {
            method:"POST",
            body: JSON.stringify(cartItems)
        })
            .then((res) => res.json())
            .then((json) => {
                console.log("json: %O", json);
            });
        // setCartItems(emptyCart());
    };

    const contextValue = {
        items,
        cartItems,
        getTotalCartAmount,
        addToCart,
        removeFromCart,
        updateCartItemCount,
        checkout,
    };
    return (
        <ShopContext.Provider value={contextValue}>
            {props.children}
        </ShopContext.Provider>
    );
};
