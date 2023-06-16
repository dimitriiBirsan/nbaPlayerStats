import NavBar from './components/Navbar';
import PlayerCard from './components/PlayerCard';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import backgroundImage from "./images/background.png";
function App() {
  return (
    <div className="App" style={{ backgroundImage:`url("${backgroundImage}")`,backgroundRepeat:"repeat", height:"100vh" }}>
      <NavBar />
      <PlayerCard />
    </div>
  );
}

export default App;
