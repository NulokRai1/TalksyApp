import React from "react";
import "../styles/Login.css";
import { useForm } from "react-hook-form";
import { LoginUser } from "../services/authService";

function Login() {
  const { register, handleSubmit } = useForm();

  const onSubmit = async (data) => {
    try {
      const result = await LoginUser(data);
      console.log(result.data);
      if (result.success) {
        alert("Success");
      } else {
        alert("Failed");
      }

      // should be "success" or error message
    } catch (err) {
      alert("Login failed");
    }
  };

  return (
    <div className="login-page">
      <div className="login-info">
        <img id="login-logo" src="/Talksy.png" alt="logo" />
        <div className="info-header">
          <p>Hello,</p>
          <p>welcome!</p>
        </div>
        <p className="sub-head">Where Conversation Feel Human Again.</p>
        <p className="info-body">
          Connect instantly with people who matter. Secure. Simple. Seamless
          messaging — all in one place.
        </p>
      </div>
      <div className="login-container">
        <h1>Welcome Back!</h1>
        <form onSubmit={handleSubmit(onSubmit)}>
          <input
            {...register("email")}
            className="login-input"
            type="text"
            placeholder="Email address or phone number"
          />

          <input
            {...register("password")}
            className="login-input"
            type="password"
            placeholder="Password"
          />

          <a href="#" id="forgot-password">
            Forgot Password?
          </a>

          <button className="login-button" type="submit">
            Login
          </button>
        </form>
        <div className="or">OR</div>
        <button className="login-google-button">
          <img className="other-link" src="/google.png" alt="Google" />
          Login with Google
        </button>
        <p>
          Don't have an account?{" "}
          <a href="#" id="signing">
            SignUp
          </a>
        </p>
      </div>
    </div>
  );
}

export default Login;
