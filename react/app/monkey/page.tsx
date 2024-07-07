"use client";
import React, { useContext } from "react";

import Image from "next/image";
import monkeyImage from "../../images/monkey.png";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import ListItemIcon from "@mui/material/ListItemIcon";
import RefreshIcon from "@mui/icons-material/Refresh";
import GavelIcon from "@mui/icons-material/Gavel";
import Grid from "@mui/material/Grid";
import Divider from "@mui/material/Divider";
import Chip from "@mui/material/Chip";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutline";
import RestartAltIcon from "@mui/icons-material/RestartAlt";
import HighlightOffIcon from "@mui/icons-material/HighlightOff";
import { green, red, pink } from "@mui/material/colors";
import { ShopContext } from "@/context/shop-context";

const Monkey = () => {
    const {
        serviceStatus,
        updateServiceStatus,
        features,
        monkeyTimer,
        updateMonkeyTimer,
        refreshStatus,
        resetStatus,
    } = useContext(ShopContext);
    var initialized: boolean = false;

    const toggleFeature = (feature: string) => {
        if (feature == "random") {
            console.log("random");
            if (monkeyTimer < 0) {
                console.log("monkeyTimer: %O", monkeyTimer);
                const id = startMonkeyTimer();
                updateMonkeyTimer(id);
                console.log("monkeyTimer: %O", id);
            } else {
                console.log("timer available");
                clearInterval(monkeyTimer);
                updateMonkeyTimer(-1);
            }
            return;
        }

        let curStatus = serviceStatus[feature];
        let newStatus = curStatus ? false : true;
        serviceStatus[feature] = newStatus;
        updateServiceStatus(feature, newStatus);
        console.log(
            "* toggle: %O from %O to %O(%O)",
            feature,
            curStatus,
            newStatus,
            serviceStatus[feature]
        );
    };

    const pullStatus = () => {
        refreshStatus();
    };

    const reset = () => {
        resetStatus();
    };

    const startMonkeyTimer = () => {
        const intervalId = setInterval(() => {
            randomMonkey();
        }, 5000);
        return intervalId;
    };

    const randomMonkey = () => {
        const trueFalses: boolean[] = [true, false];
        const feature = features[Math.floor(Math.random() * features.length)];
        toggleFeature(feature);
    };

    return (
        <>
            <Grid container spacing={2}>
                <Grid item xs={8}>
                    <Divider>
                        <Chip label="Break/Fix Actions" size="small" />
                    </Divider>

                    <List>
                        <ListItemButton onClick={() => toggleFeature("random")}>
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            {monkeyTimer < 0 ? (
                                <ListItemText primary="Start Chaos monkey (Random Break)" />
                            ) : (
                                <ListItemText primary="Stop Chaos monkey" />
                            )}
                        </ListItemButton>
                        <ListItemButton onClick={() => toggleFeature("login")}>
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Fix Login" />
                        </ListItemButton>
                        <ListItemButton
                            onClick={() => toggleFeature("products")}
                        >
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Fix Product List" />
                        </ListItemButton>
                        <ListItemButton
                            onClick={() => toggleFeature("checkout")}
                        >
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Fix Checkout" />
                        </ListItemButton>
                        <ListItemButton
                            onClick={() => toggleFeature("delivery")}
                        >
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Fix Delivery" />
                        </ListItemButton>
                        <ListItemButton onClick={() => reset()}>
                            <ListItemIcon>
                                <RestartAltIcon />
                            </ListItemIcon>
                            <ListItemText primary="Reset All Status" />
                        </ListItemButton>
                        <ListItemButton onClick={() => pullStatus()}>
                            <ListItemIcon>
                                <RefreshIcon />
                            </ListItemIcon>
                            <ListItemText primary="Pull Latest Status" />
                        </ListItemButton>
                    </List>

                    <Divider>
                        <Chip label="Status" size="small" />
                    </Divider>
                    <List>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["login"] ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : (
                                    <HighlightOffIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Login Service" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["products"] ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : (
                                    <HighlightOffIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Products Service" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["checkout"] ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : (
                                    <HighlightOffIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Checkout Service" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["delivery"] ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : (
                                    <HighlightOffIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Delivery Service" />
                        </ListItem>
                    </List>
                </Grid>

                <Grid item xs={4}>
                    <Image src={monkeyImage} width={300} alt="Chaos Monkey" />
                </Grid>
            </Grid>
        </>
    );
};

export default Monkey;
