import React, { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';

function LoginModal({ showModal, handleLogin }) {
    const [email, setEmail] = useState("");

    return (
        <Modal show={showModal} onHide={handleLogin}>
            <Modal.Header closeButton>
                <Modal.Title>Login</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group controlId="formBasicEmail">
                        <Form.Label>Email address</Form.Label>
                        <Form.Control type="email" placeholder="Enter email" value={email} onChange={(e) => setEmail(e.target.value)} />
                        <Form.Text className="text-muted">
                            We'll send a login link to your email.
                        </Form.Text>
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleLogin}>
                    Close
                </Button>
                <Button variant="primary" onClick={() => handleLogin(email)}>
                    Login
                </Button>
            </Modal.Footer>
        </Modal>
    );
}

export default LoginModal;