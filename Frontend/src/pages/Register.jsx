import React from 'react';
import '../styles/Login.css';
import { useNavigate } from 'react-router-dom';



function Register() {

    const navigate = useNavigate();

    const handleSubmit = () => {
        navigate('/login')
    }
    return (
        <div className='login-page'>
            <div className="login-info">
                <img id='login-logo' src="/Talksy.png" alt="logo" />
                <div className="info-header">
                    <p>Get Started with Talksy.</p>
                </div>
                <div className="sub-head">
                    <p>Answer a couple of questions to get started with Talksy.</p>
                </div>
            </div>

            <form className="register-container" onSubmit={handleSubmit}>
                <button className='login-button'><img className="other-link" src="/google.png" alt="Google" />Sign In with Google</button>
                <div className="or">OR</div>
                <input className='login-input' type="text" placeholder='Email address or phone number' required />
                <input className='login-input' type="password" placeholder='Password' required />
                <input className='login-input' type="password" placeholder='Confirm Password' required />
                <input className='login-input' type="text" placeholder='Full Name' required />
                <input className='login-input' type="text" placeholder='UserName' required />
                <button className='login-button' >Sign Up</button>
                <p>
                    Have an account? <a href="#" id='signing'>Log In</a>
                </p>

            </form>
        </div>
    )
}

export default Register
