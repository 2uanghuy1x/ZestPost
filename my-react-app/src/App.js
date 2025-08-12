import React, { useState } from 'react';
import { BrowserRouter, Route, Routes, Link } from 'react-router-dom';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <div className="App">
                <nav>
                    <Link to="/">Tài khoản</Link> | <Link to="/details">Chi tiết</Link> | <Link to="/settings">Cài đặt</Link>
                </nav>
                <Routes>
                    <Route path="/" element={<AccountScreen />} />
                    <Route path="/details" element={<DetailsScreen />} />
                    <Route path="/settings" element={<SettingsScreen />} />
                </Routes>
            </div>
        </BrowserRouter>
    );
}

function AccountScreen() {
    const accounts = [
        { id: 1, name: 'Account 1' },
        { id: 2, name: 'Account 2' },
        { id: 3, name: 'Account 3' },
    ];
    const [selectedAccounts, setSelectedAccounts] = useState([]);

    const handleCheckboxChange = (account) => {
        setSelectedAccounts((prev) => {
            const isSelected = prev.some((acc) => acc.id === account.id);
            return isSelected
                ? prev.filter((acc) => acc.id !== account.id)
                : [...prev, account];
        });
    };

    const handleStartClick = () => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(
                JSON.stringify({
                    screen: 'accounts',
                    data: selectedAccounts, // Gửi toàn bộ đối tượng { id, name }
                })
            );
        } else {
            console.log("WebView2 not available");
        }
    };

    return (
        <div>
            <h1>Chọn tài khoản</h1>
            <table>
                <thead>
                    <tr>
                        <th>Chọn</th>
                        <th>ID</th>
                        <th>Tên tài khoản</th>
                    </tr>
                </thead>
                <tbody>
                    {accounts.map((account) => (
                        <tr key={account.id}>
                            <td>
                                <input
                                    type="checkbox"
                                    checked={selectedAccounts.some((acc) => acc.id === account.id)}
                                    onChange={() => handleCheckboxChange(account)}
                                />
                            </td>
                            <td>{account.id}</td>
                            <td>{account.name}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            <button onClick={handleStartClick}>Start</button>
        </div>
    );
}

function DetailsScreen() {
    const handleSendDetails = () => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(
                JSON.stringify({
                    screen: 'details',
                    data: { accountId: '123', accountName: 'Sample Account' },
                })
            );
        }
    };

    return (
        <div>
            <h1>Chi tiết tài khoản</h1>
            <button onClick={handleSendDetails}>Gửi chi tiết</button>
        </div>
    );
}

function SettingsScreen() {
    const handleSaveSettings = () => {
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.postMessage(
                JSON.stringify({
                    screen: 'settings',
                    data: { theme: 'dark', notifications: true },
                })
            );
        }
    };

    return (
        <div>
            <h1>Cài đặt</h1>
            <button onClick={handleSaveSettings}>Lưu cài đặt</button>
        </div>
    );
}

export default App;