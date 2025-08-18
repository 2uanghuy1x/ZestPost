import React, { useState } from 'react';
import AccountList from '../../components/AccountManagement/AccountList';
import './JoinGroup.css';

const JoinGroup = () => {
    const [groupLink, setGroupLink] = useState('');
    const [joinDelay, setJoinDelay] = useState(5);

    const handleGroupLinkChange = (e) => {
        setGroupLink(e.target.value);
    };

    const handleJoinDelayChange = (e) => {
        setJoinDelay(e.target.value);
    };

    const handleSubmit = () => {
        console.log("Selected Accounts:", "Need to get from AccountList");
        console.log("Group Link:", groupLink);
        console.log("Join Delay:", joinDelay);
        // Implement join group logic here
    };

    return (
        <div className="join-group-container">
            <h2>Tham gia nhóm</h2>

            <div className="section">
                <h3>Danh sách tài khoản</h3>
                <AccountList />
            </div>

            <div className="section">
                <h3>Cấu hình tham gia nhóm</h3>
                <div className="form-group">
                    <label>Link nhóm (mỗi link một dòng):</label>
                    <textarea 
                        value={groupLink} 
                        onChange={handleGroupLinkChange} 
                        rows="5"
                        placeholder="Nhập các link nhóm vào đây..."
                    ></textarea>
                </div>
                <div className="form-group">
                    <label>Độ trễ khi tham gia (giây):</label>
                    <input 
                        type="number" 
                        value={joinDelay} 
                        onChange={handleJoinDelayChange} 
                    />
                </div>
            </div>

            <button onClick={handleSubmit} className="submit-button">Bắt đầu tham gia nhóm</button>
        </div>
    );
};

export default JoinGroup;
