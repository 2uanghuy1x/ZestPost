
import React, { useState } from 'react';
import Accordion from '../Accordion/Accordion';
import '../Accordion/Accordion.css';
import './Settings.css';

const Settings = () => {
    const [backupPath, setBackupPath] = useState('');
    const [numThreads, setNumThreads] = useState(5);
    const [actionDelay, setActionDelay] = useState(3);
    const [cookieInput, setCookieInput] = useState('');
    const [loginMethod, setLoginMethod] = useState('cookie');
    const [language, setLanguage] = useState('vi');

    const handleBackup = () => {
        console.log("Backing up data to:", backupPath);
        alert("Chức năng sao lưu dữ liệu sẽ được triển khai tại đây.");
    };

    const handleRestore = () => {
        console.log("Restoring data from:", backupPath);
        alert("Chức năng khôi phục dữ liệu sẽ được triển khai tại đây.");
    };

    return (
        <div className="settings-container">
            <h2>Cài đặt</h2>

            <Accordion title="Sao lưu / Khôi phục dữ liệu">
                <div className="form-group">
                    <label>Đường dẫn sao lưu:</label>
                    <input 
                        type="text" 
                        value={backupPath} 
                        onChange={(e) => setBackupPath(e.target.value)} 
                        placeholder="E.g., C:/ZestPost_Backup"
                    />
                </div>
                <div className="button-group">
                    <button onClick={handleBackup} className="setting-button primary">Sao lưu</button>
                    <button onClick={handleRestore} className="setting-button secondary">Khôi phục</button>
                </div>
            </Accordion>

            <Accordion title="Cấu hình hoạt động">
                <div className="form-group">
                    <label>Số luồng:</label>
                    <input 
                        type="number" 
                        value={numThreads} 
                        onChange={(e) => setNumThreads(e.target.value)}
                        min="1"
                    />
                </div>
                <div className="form-group">
                    <label>Thời gian dừng khi thực hiện hành động (giây):</label>
                    <input 
                        type="number" 
                        value={actionDelay} 
                        onChange={(e) => setActionDelay(e.target.value)}
                        min="0"
                    />
                </div>
            </Accordion>

            <Accordion title="Kiểm tra tài khoản">
                <div className="form-group">
                    <label>Cookie để kiểm tra tài khoản:</label>
                    <textarea 
                        value={cookieInput} 
                        onChange={(e) => setCookieInput(e.target.value)}
                        rows="4"
                        placeholder="Nhập cookie vào đây..."
                    ></textarea>
                </div>
                <button className="setting-button primary" onClick={() => alert("Kiểm tra tài khoản bằng cookie sẽ được triển khai tại đây.")}>Kiểm tra Cookie</button>
            </Accordion>

            <Accordion title="Tùy chọn đăng nhập">
                <div className="form-group">
                    <label>Phương thức đăng nhập mặc định:</label>
                    <select value={loginMethod} onChange={(e) => setLoginMethod(e.target.value)}>
                        <option value="cookie">Đăng nhập bằng Cookie</option>
                        <option value="credentials">Đăng nhập bằng thông tin tài khoản</option>
                    </select>
                </div>
            </Accordion>

            <Accordion title="Ngôn ngữ">
                <div className="form-group">
                    <label>Chọn ngôn ngữ:</label>
                    <select value={language} onChange={(e) => setLanguage(e.target.value)}>
                        <option value="vi">Tiếng Việt</option>
                        <option value="en">English</option>
                    </select>
                </div>
            </Accordion>

            <button className="save-settings-button">Lưu cài đặt</button>
        </div>
    );
};

export default Settings;
