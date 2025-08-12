import React, { useState } from 'react';
import './App.css'; // Sẽ cập nhật CSS sau
import AccountCrud from './AccountCrud';
import CategoryCrud from './CategoryCrud';

function App() {
  const [activeView, setActiveView] = useState('accounts');

  const renderView = () => {
    switch (activeView) {
      case 'accounts':
        return <AccountCrud />;
      case 'categories':
        return <CategoryCrud />;
      default:
        return <AccountCrud />;
    }
  };

  return (
    <div className="app-container">
      <div className="sidebar">
        <div className="sidebar-header">
          <h2>ZestPost</h2>
        </div>
        <ul className="nav-menu">
          <li 
            className={activeView === 'accounts' ? 'active' : ''}
            onClick={() => setActiveView('accounts')}
          >
            Quản lý Tài khoản
          </li>
          <li 
            className={activeView === 'categories' ? 'active' : ''}
            onClick={() => setActiveView('categories')}
          >
            Quản lý Danh mục
          </li>
          {/* Thêm các mục menu khác ở đây */}
        </ul>
      </div>
      <div className="main-content">
        {renderView()}
      </div>
    </div>
  );
}

export default App;
