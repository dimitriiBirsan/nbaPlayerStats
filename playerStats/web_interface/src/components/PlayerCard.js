import React, { useEffect, useState } from 'react';
import { Card, Row, Col } from 'react-bootstrap';
import axios from 'axios';
import { FaRegHeart, FaHeart } from 'react-icons/fa';
import LoginModal from './LoginModal';

function PlayerCard() {
  const [loggedIn, setLoggedIn] = useState(false);
  const [players, setPlayers] = useState([]);
  const [showModal, setShowModal] = useState(false);


  useEffect(() => {
    const email = localStorage.getItem('email');
    let data = {}
    console.log(email)
    if(email){
      data.email = email
    }
    axios.get('/api/GetTrendiestPlayer',{params: data}) 
      .then(response => {
        setPlayers(response.data);
      })
      .catch(error => {
        console.error('There was an error!', error);
      });
  }, []);

  const handleLogin = (email) => {
    if (email) {

      localStorage.setItem('email', email);
      axios.post("/api/Login", {email})
        .then(response => {
          setLoggedIn(true);

          // Fetch players data from your backend API
          axios.get('/api/GetTrendiestPlayer') // Replace this with your actual API endpoint
            .then(response => {
              setPlayers(response.data);
            })
            .catch(error => {
              console.error('There was an error!', error);
            });

        })
        .catch((error) => console.log(error) )
      // Set the user as logged in
    }

    // Close the modal
    setShowModal(false);
  };

  const handleFavoriteClick = (playerId, isFavorited) => {
    const email = localStorage.getItem('email');
    if (!loggedIn && !email ) {
      axios.get("/api/checkIfUserIsLoggedIn")
        .then( (response) => {
          setLoggedIn(true);
          console.log(response)
        } ).catch(e => setShowModal(true))
      return
    }else {
      setLoggedIn(true)
    }
    axios({
      method: 'post',
      url: '/api/AddFavoritePlayer',
      data: { id:playerId, email },
      withCredentials: true
    }).then(response => {
        setPlayers(players.map(player =>
          player.player.id === playerId
            ? { ...player, isFavorited: !isFavorited }
            : player
        ));
      })
      .catch(error => {
        console.error('There was an error!', error);
      });

  };

  return (
    <>
    <LoginModal showModal={showModal} handleLogin={handleLogin} />
    <Row>
      {players.map((player, index) => {
        let borderColor, headerColor;
        if (index < 3) {
          borderColor = 'gold';
          headerColor = 'goldenrod';
        } else if (index < 6) {
          borderColor = 'silver';
          headerColor = 'lightgray';
        } else {
          borderColor = '#cd7f32'; // Bronze color
          headerColor = '#cd7f32';
        }
        return (
          <Col key={index} xs={12} sm={6} md={4} lg={3} xl={2} >
            <Card key={index} style={{ width: '100%', margin: '10px', borderColor: borderColor, borderWidth: 2, textAlign: "start" }}>
              <Card.Header style={{ backgroundColor: headerColor, color: 'white', fontSize: "1.2rem" }}>

                <span style={{ marginRight: '20px', marginLeft: 0, padding: '10px', border: '2px solid white', borderRadius: '5px' }}>
                  {index + 1}
                </span>
                <span >
                  {player.player.first_name} {player.player.last_name}

                </span>
                <button style={{ background: 'none', border: 'none', color: 'white', marginLeft: "auto" }} onClick={() => handleFavoriteClick(player.player.id, player.isFavorited)}>
                  {player.isFavorited ? <FaHeart /> : <FaRegHeart />}
                </button>

              </Card.Header>
              <Card.Body >
                <p>Average Points: {player.averagePoints}</p>
                <p>Team: {player.teams.full_name}</p>
                <p>Height: {player.player.height_feet} feet {player.player.height_inches} inches</p>
                <p>Weight: {player.player.weight_pounds} pounds</p>
                <p>Division: {player.teams.division}</p>
              </Card.Body>
            </Card>
          </Col>
        )
      })}
    </Row>
    </>
  );
}

export default PlayerCard;