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
import { green, red, orange, pink } from "@mui/material/colors";
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
        let newStatus = (curStatus + 1) % 3;
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
                        <Chip label="Break/Slow/Fix Actions" size="small" />
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
                            <ListItemText primary="Break/Slow/Fix Login" />
                        </ListItemButton>
                        <ListItemButton
                            onClick={() => toggleFeature("products")}
                        >
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Slow/Fix Product List" />
                        </ListItemButton>
                        <ListItemButton
                            onClick={() => toggleFeature("checkout")}
                        >
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Slow/Fix Checkout" />
                        </ListItemButton>
                        <ListItemButton
                            onClick={() => toggleFeature("delivery")}
                        >
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break/Slow/Fix Delivery" />
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
                                {serviceStatus["login"] === 0 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : serviceStatus["login"] === 2 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: orange[500] }}
                                    />
                                ) : (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Login Service" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["products"] === 0 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : serviceStatus["products"] === 2 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: orange[500] }}
                                    />
                                ) : (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Products Service" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["checkout"] === 0 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : serviceStatus["checkout"] === 2 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: orange[500] }}
                                    />
                                ) : (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Checkout Service" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                {serviceStatus["delivery"] === 0 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: green[500] }}
                                    />
                                ) : serviceStatus["delivery"] === 2 ? (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: orange[500] }}
                                    />
                                ) : (
                                    <CheckCircleOutlineIcon
                                        sx={{ color: red[500] }}
                                    />
                                )}
                            </ListItemIcon>
                            <ListItemText primary="Delivery Service" />
                        </ListItem>
                    </List>
                    <Divider>
                        <Chip label="Legend" size="small" />
                    </Divider>
                    <List>
                        <ListItem>
                            <ListItemIcon>
                                <CheckCircleOutlineIcon
                                    sx={{ color: green[500] }}
                                />
                            </ListItemIcon>
                            <ListItemText primary="Operational" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                <CheckCircleOutlineIcon
                                    sx={{ color: orange[500] }}
                                />
                            </ListItemIcon>
                            <ListItemText primary="Slow" />
                        </ListItem>
                        <ListItem>
                            <ListItemIcon>
                                <CheckCircleOutlineIcon
                                    sx={{ color: red[500] }}
                                />
                            </ListItemIcon>
                            <ListItemText primary="Broken" />
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
