import Image from "next/image";
import monkeyImage from "../../images/monkey.png";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import ListItemIcon from "@mui/material/ListItemIcon";
import HardwareIcon from "@mui/icons-material/Hardware";
import GavelIcon from "@mui/icons-material/Gavel";
import Grid from "@mui/material/Grid";

const Monkey = () => {
    return (
        <>
            <h1>Chaos Monkey: TBD </h1>
            <Grid container cpacing={2}>
                <Grid item xs={8}>
                    <List>
                        <ListItemButton component="a" href="#simple-list">
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Chaos (Random Break)" />
                        </ListItemButton>
                        <ListItemButton component="a" href="#simple-list">
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break Login" />
                        </ListItemButton>
                        <ListItemButton component="a" href="#simple-list">
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break Product List" />
                        </ListItemButton>
                        <ListItemButton component="a" href="#simple-list">
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break Checkout" />
                        </ListItemButton>
                        <ListItemButton component="a" href="#simple-list">
                            <ListItemIcon>
                                <GavelIcon />
                            </ListItemIcon>
                            <ListItemText primary="Break Delivery" />
                        </ListItemButton>
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
