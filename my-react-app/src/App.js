
import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import AccountCrud from './AccountCrud';
import CategoryCrud from './CategoryCrud';
import PostArticle from './PostArticle'; // Import the new component
import './App.css';

function App() {
  return (
    <Router>
      <div>
        <nav>
          <ul>
            <li>
              <Link to="/accounts">Quản lý tài khoản</Link>
            </li>
            <li>
              <Link to="/categories">Quản lý danh mục</Link>
            </li>
            <li>
              <Link to="/post-article">Đăng bài viết</Link> 
            </li>
          </ul>
        </nav>

        <hr />

        <Routes>
          <Route path="/accounts" element={<AccountCrud />} />
          <Route path="/categories" element={<CategoryCrud />} />
          <Route path="/post-article" element={<PostArticle />} /> 
        </Routes>
      </div>
    </Router>
  );
}

export default App;
