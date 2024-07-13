"use client";
import { createContext, useEffect, useState } from "react";

// global props with the default values
export const ShopContext = createContext(null);

// const baseCsUrl = "http://10.1.196.5:15001";
const baseCsUrl = "http://scottmm.local:15001";
const productUrl = baseCsUrl + "/products/";
const checkoutUrl = baseCsUrl + "/checkout/";
const loginUrl = baseCsUrl + "/login/";
const serviceStatusUrl = baseCsUrl + "/status/";
const serviceBreakUrl = baseCsUrl + "/break/";
const serviceSlowUrl = baseCsUrl + "/slow/";
const serviceFixUrl = baseCsUrl + "/fix/";
const resetStatusUrl = baseCsUrl + "/reset/";
const features: string[] = ["login", "products", "checkout", "delivery"];
const monkeyTimer: number = -1;

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
        const stored = localStorage.getItem("cartItems");
        return stored ? JSON.parse(stored) : items;
    }
    console.log("getCart: window NOT available, using the default empty cart");
    return items;
};

const getLoginInfo = () => {
    let loginInfo = {};

    if (typeof window !== "undefined") {
        const stored = localStorage.getItem("loginInfo");
        return stored ? JSON.parse(stored) : loginInfo;
    }
    console.log(
        "getLoginInfo: window NOT available, using the empty login info"
    );
    return loginInfo;
};

const getServiceStatus = () => {
    let serviceStatus = {
        login: 0,
        products: 0,
        checkout: 0,
        delivery: 0,
    };

    if (typeof window !== "undefined") {
        const stored = localStorage.getItem("serviceStatus");
        return stored ? JSON.parse(stored) : serviceStatus;
    }
    console.log(
        "getServiceStatus: window NOT available, using the default service status"
    );
    return serviceStatus;
};

var initialized: boolean = false;

export const ShopContextProvider = (props) => {
    // *** states ***
    // state: items (products)
    const [items, setItems] = useState([]);

    useEffect(() => {
        if (initialized === false) {
            initialized = true;
            // fetch products
            console.log("fetching products from %O", productUrl);
            fetch(productUrl)
                .then((res) => res.json())
                .then((json) => {
                    setItems(json);
                });

            // fetch service status
            refreshStatus();
        }
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

    // state: serviceStatus
    const [serviceStatus, setServiceStatus] = useState(getServiceStatus());
    // save serviceStatus in localStorage whenever it changes
    useEffect(() => {
        console.log("saving serviceStatus %O in localStorage", serviceStatus);
        localStorage.setItem("serviceStatus", JSON.stringify(serviceStatus));
    }, [serviceStatus]);

    // state: monkeyTimer
    const [monkeyTimer, setMonkeyTimer] = useState(-1);

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
            cartItems: cartItems,
            username: loginInfo.username,
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

    const login = async (username: string): Promise<boolean> => {
        console.log("login(%O)", username);
        const resp = await fetch(loginUrl + "?username=" + username);
        if (!resp.ok) {
            return false;
        }

        const json = await resp.json();
        if (json.result === "Success") {
            console.log("login(%O) succeeded", username);
            setLoginInfo((prev) => ({
                ...prev,
                loggedIn: true,
                username: username,
            }));
            return true;
        } else {
            console.log("login(%O) failed", username);
            return false;
        }
    };

    const logout = () => {
        console.log("logout");
        setLoginInfo((prev) => ({ ...prev, loggedIn: false, username: "" }));
    };

    const updateServiceStatus = (feature: string, status: number) => {
        console.log("updateService status %O=%O", feature, status);
        setServiceStatus((prev) => ({ ...prev, feature: status }));

        if (status === 0) {
            fetch(serviceFixUrl + feature).then((res) => {
                console.log("service fix returned %O", res);
            });
        } else if (status === 1) {
            fetch(serviceBreakUrl + feature).then((res) => {
                console.log("service break returned %O", res);
            });
        } else {
            fetch(serviceSlowUrl + feature).then((res) => {
                console.log("service slow returned %O", res);
            });
        }
    };

    const updateMonkeyTimer = (id: number) => {
        console.log("updateMonkeyTimer(%O)", id);
        setMonkeyTimer(id);
    };

    const refreshStatus = () => {
        console.log("fetching service status from %O", serviceStatusUrl);
        fetch(serviceStatusUrl)
            .then((res) => res.json())
            .then((json) => {
                setServiceStatus(json);
            });
    };

    const resetStatus = () => {
        console.log("resetStatus");
        fetch(resetStatusUrl)
            .then((res) => res.json())
            .then((json) => {
                setServiceStatus(json);
            });
    };

    const contextValue = {
        items,
        cartItems,
        loginInfo,
        serviceStatus,
        features,
        monkeyTimer,
        getTotalCartAmount,
        addToCart,
        removeFromCart,
        updateCartItemCount,
        clearCart,
        checkout,
        login,
        logout,
        updateServiceStatus,
        updateMonkeyTimer,
        refreshStatus,
        resetStatus,
    };
    return (
        <ShopContext.Provider value={contextValue}>
            {props.children}
        </ShopContext.Provider>
    );
};
