import React from 'react';
import logo from "../images/website_logo.png"
import { Navbar, Nav } from 'react-bootstrap';

const NavBar = () => {
    return (
        <Navbar bg="dark" variant="dark">
            <Navbar.Brand href="/">
              <img src={logo} className="navbar-logo" alt="logo" />
            </Navbar.Brand>

            <Nav className="mr-auto">
                <Nav.Link href="/favorite">Favorite Players</Nav.Link>
            </Nav>
        </Navbar>
    );
}

export default NavBar;